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
		static State get_lowest_cost(List<State> list) {
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

		public static List<State> a_star(Grid grid, State start_state, State target_state) {
			List<State> open_list = new List<State>();
			HashSet<State> closed_list = new HashSet<State>();

			State current_state;
			List<State> neighbouring_states;

			float cost;
			float heuristic;
			bool is_in_open;

			int iteration_count = 0; // for testing only.
			Predicate<State> stateFinder;

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
				
				neighbouring_states = grid.expand_state(current_state);

				for (int i = 0; i < neighbouring_states.Count; i++) {
					if (!closed_list.Contains(neighbouring_states[i])) {
						cost = current_state.cost + state_heuristic(neighbouring_states[i], current_state);
						is_in_open = open_list.Contains(neighbouring_states[i]);
						
						if (is_in_open) {
							stateFinder = (State state) => { return state.Equals(neighbouring_states[i]); };
							neighbouring_states[i] = open_list.Find(stateFinder);
						}

						/**
						 * Because the State objects are generated and not in a preset list all neighbouring states will
						 * always have a cost of 0, therefor the comparison cost less than state.cost will never be true.
						 * This means the selected path is not necessarily the shortest.
						 * 
						 * We might need to precompute all states and all heuristics.
						 **/

						// Console.WriteLine("cost: " + neighbouring_states[i].cost);

						if (cost < neighbouring_states[i].cost || !is_in_open) {
							heuristic = state_heuristic(target_state, neighbouring_states[i]);
							neighbouring_states[i].set_cost(cost, heuristic);
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
