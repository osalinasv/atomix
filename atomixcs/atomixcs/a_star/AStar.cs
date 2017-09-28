using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace atomixcs.a_star {
	class AStar {
		static float manhattan_distance(Node a, Node b) {
			return Math.Abs(a.position.x - b.position.x) + Math.Abs(a.position.y - b.position.y);
		}

		/**
		 * @Optimization: what would be a way of differentiating a closer to solved state from an unsolved/unoptimal state
		 * represented just by a real number?
		 * 
		 * Adding the distance between the atoms themselves hindered the results in more cases than those in which it did help,
		 * considering how close are the atoms from each other made the algorithm prefer paths that led to grouping the atoms
		 * disregarding if they are actually close to the goal.
		 **/
		static float state_heuristic(State current, State target) {
			float heuristic = 0;

			for (int i = 0; i < current.items.Count && i < target.items.Count; i++) {
				heuristic += manhattan_distance(current.items[i], target.items[i]);
			}

			return heuristic;
		}

		/**
		 * @Optimization: replace this with proper priority queue so we can pop the lowest cost item more efficiently.
		 * 
		 * --Note: tried implementing this PriorityQueue: https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
		 * but the results where x10 times slower than with this simple function, going from ~250 iterations to ~3700
		 * on the same example.
		 **/
		static State get_lowest_cost(HashSet<State> list) {
			State current = null;

			foreach (State state in list) {
				if (current == null || state.f_cost < current.f_cost) {
					current = state;
				}
			}

			return current;
		}

		static List<State> reconstruct_path(State current_state, State start_state, State target_state) {
			List<State> path = new List<State>();
			State current = current_state;

			if (current_state.Equals(target_state)) {
				while (current != null) {
					path.Add(current);
					current = current.previous;
				}

				if (path.Count > 0) {
					path.Reverse();
				}
			}

			return path;
		}

		static List<State> precompute_states(Grid grid, State start_state, State target_state) {
			List<State> visited_states = new List<State>();
			List<State> states = new List<State>();
			List<State> neighbours;
			State current;

			states.Add(start_state);

			while (states.Count > 0) {
				current = states[0];
				states.Remove(current);

				if (!visited_states.Contains(current)) {
					current.heuristic = state_heuristic(current, target_state);
					visited_states.Add(current);
				}

				neighbours = grid.expand_state(current);

				foreach (State neighbour in neighbours) {
					if (!states.Contains(neighbour)) {
						states.Add(neighbour);
						current.neighbours.Add(neighbour);
					}
				}

				if (target_state.Equals(current)) {
					break;
				}
			}
			
			return visited_states;
		}

		public static List<State> a_star(Grid grid, State start_state, State target_state) {
			HashSet<State> open_list = new HashSet<State>();
			HashSet<State> closed_list = new HashSet<State>();

			State current_state;
			List<State> neighbouring_states;

			float cost;
			bool is_in_open;

			int iteration_count = 0; // for testing only.

			Console.WriteLine("Precomputing...");
			List<State> all_states = precompute_states(grid, start_state, target_state);
			Console.WriteLine("Found " + all_states.Count + " usable states");

			Stopwatch watch = new Stopwatch();
			watch.Start();

			start_state.set_cost(0, state_heuristic(start_state, target_state));
			open_list.Add(start_state);

			while (open_list.Count > 0) {
				iteration_count++;

				current_state = get_lowest_cost(open_list);
				open_list.Remove(current_state);
				closed_list.Add(current_state);

				if (current_state.Equals(target_state)) {
					watch.Stop();

					Console.WriteLine("\n==============================================\n");
					Console.WriteLine("\nEND state:");

					grid.draw_grid(current_state);
					Console.WriteLine(current_state);
					Console.WriteLine();

					Console.WriteLine("Finished in: {0} iterations | {1} ms\n", iteration_count, watch.ElapsedMilliseconds);
					return reconstruct_path(current_state, start_state, target_state);
				}
				
				neighbouring_states = current_state.neighbours;

				for (int i = 0; i < neighbouring_states.Count; i++) {
					if (!closed_list.Contains(neighbouring_states[i])) {
						cost = current_state.cost + state_heuristic(neighbouring_states[i], current_state);
						is_in_open = open_list.Contains(neighbouring_states[i]);

						if (cost < neighbouring_states[i].cost || !is_in_open) {
							neighbouring_states[i].set_cost(cost, neighbouring_states[i].heuristic);
							neighbouring_states[i].previous = current_state;

							if (!is_in_open) {
								open_list.Add(neighbouring_states[i]);
							}
						}
					}
				}
			}

			watch.Stop();

			return null;
		}
	}
}
