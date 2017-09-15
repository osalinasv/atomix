using System;
using System.Collections.Generic;

namespace atomixcs.a_star {
	class AStar {
		static float manhattan_heuristic(Node a, Node b) {
			return Math.Abs(a.position.x - b.position.x) + Math.Abs(a.position.y - b.position.y);
		}

		static float state_heuristic(State current, State target) {
			float heuristic = 0;

			for (int i = 0; i < current.items.Count && i < target.items.Count; i++) {
				heuristic += manhattan_heuristic(current.items[i], target.items[i]);
			}

			return heuristic;
		}

		// @Important: replace this with proper priority queue se we can pop the lowest cost item.
		static State get_lowest_cost(List<State> list) {
			State node = list[0];

			for (int i = 1; i < list.Count; i++) {
				if (list[i].f_cost < node.f_cost) {
					node = list[i];
				}
			}

			return node;
		}

		static bool compare_nodes(Node a, Node b) {
			return a.position == b.position;
		}

		static bool compare_state(State a, State b) {
			for (int i = 0; i < a.items.Count && i < b.items.Count; i++) {
				if (!compare_nodes(a.items[i], b.items[i])) {
					return false;
				}
			}

			return true;
		}

		static bool contains_state(List<State> list, State current) {
			foreach (State state in list) {
				if (compare_state(state, current)) {
					return true;
				}
			}

			return false;
		}
		
		public static List<State> a_star(Grid grid, State start_state, State target_state) {
			List<State> open_list = new List<State>();
			HashSet<State> closed_list = new HashSet<State>();

			List<State> path = new List<State>();

			State current_state = null;
			List<State> neighbouring_states = null;

			float cost = 0;
			float heuristic = 0;
			bool is_in_open = false;

			start_state.set_cost(0, state_heuristic(start_state, target_state));
			open_list.Add(start_state);

			while (open_list.Count > 0) {
				current_state = get_lowest_cost(open_list);
				open_list.Remove(current_state);
				closed_list.Add(current_state);

				if (compare_state(current_state, target_state)) {
					return path;
				}

				neighbouring_states = grid.expand_state(current_state, target_state);

				foreach (State neighbour in neighbouring_states) {
					if (closed_list.Contains(neighbour)) {
						continue;
					}

					cost = current_state.cost + state_heuristic(current_state, neighbour);
					is_in_open = contains_state(open_list, neighbour);

					if (cost < neighbour.cost || !is_in_open) {
						heuristic = state_heuristic(current_state, target_state);
						neighbour.set_cost(cost, heuristic);

						// @Error: we need a way of storing states and reconstructing a path.
						// Currenlty the path list stores all states visited, not only the ones that are part of the solution.
						path.Add(current_state);

						if (!is_in_open) {
							open_list.Add(neighbour);
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
