using System;
using System.Collections.Generic;

namespace atomixcs.a_star {
	class AStar {
		static float manhattan_distance(Node a, Node b) {
			return Math.Abs(a.position.x - b.position.x) + Math.Abs(a.position.y - b.position.y);
		}

		// @Optimization: it might be a good idea to include the distance between atoms in the heuristic.
		// The shorter the distance the closer to an answer.
		static float state_heuristic(State current, State target) {
			float heuristic = 0;

			for (int i = 0; i < current.items.Count && i < target.items.Count; i++) {
				heuristic += manhattan_distance(current.items[i], target.items[i]);
			}

			return heuristic;
		}

		// @Optimization: replace this with proper priority queue so we can pop the lowest cost item more efficiently.
		// --Note: tried implementing this PriorityQueue: https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
		// but the results where x10 times slower than with this simple function, groing from ~250 iterations to ~3700
		// on the same example.
		static State get_lowest_cost(List<State> list) {
			State node = list[0];

			for (int i = 1; i < list.Count; i++) {
				if (list[i].f_cost < node.f_cost) {
					node = list[i];
				}
			}

			return node;
		}

		static bool compare_state(State a, State b) {
			for (int i = 0; i < a.items.Count && i < b.items.Count; i++) {
				if (a.items[i].position != b.items[i].position) {
					return false;
				}
			}

			return true;
		}

		static bool contains_state(List<State> list, State current) {
			for (int i = 0; i < list.Count; i++) {
				if (compare_state(list[i], current)) {
					return true;
				}
			}

			return false;
		}
		
		public static List<State> a_star(Grid grid, State start_state, State target_state) {
			List<State> open_list = new List<State>();
			HashSet<State> closed_list = new HashSet<State>();

			List<State> path = new List<State>();

			State current_state;
			List<State> neighbouring_states;

			float cost;
			float heuristic;
			bool is_in_open;

			start_state.set_cost(0, state_heuristic(start_state, target_state));
			open_list.Add(start_state);

			int iteration_count = 0; // for testing only.

			while (open_list.Count > 0) {
				iteration_count++;
				
				current_state = get_lowest_cost(open_list);
				open_list.Remove(current_state);
				closed_list.Add(current_state);

				if (compare_state(current_state, target_state)) {
					Console.WriteLine("\n==============================================\n");
					Console.WriteLine("\nEND state:");

					grid.draw_grid(current_state);
					Console.WriteLine(current_state);
					Console.WriteLine();

					Console.WriteLine("Finished in: {0} iterations\n", iteration_count);
					return path;
				}

				// @Optimization: find a better way of detecting usable neighbour states.
				neighbouring_states = grid.expand_state(current_state);

				for (int i = 0; i < neighbouring_states.Count; i++) {
					if (closed_list.Contains(neighbouring_states[i])) {
						continue;
					}

					cost = current_state.cost + state_heuristic(current_state, neighbouring_states[i]);
					is_in_open = contains_state(open_list, neighbouring_states[i]);

					if (cost < neighbouring_states[i].cost || !is_in_open) {
						heuristic = state_heuristic(current_state, target_state);
						neighbouring_states[i].set_cost(cost, heuristic);

						// @Important: we need a way of storing states and reconstructing a path.
						// Currenlty the path list stores all states visited, not only the ones that are part of the solution.
						path.Add(current_state);

						if (!is_in_open) {
							open_list.Add(neighbouring_states[i]);
						}
					}
				}

				grid.draw_grid(current_state);
				Console.WriteLine(current_state);
				Console.WriteLine();
			}

			return path;
		}
	}
}
