using System;
using System.Collections.Generic;
using System.Diagnostics;

using Priority_Queue;

namespace atomixcs.a_star {
	class AStar {
		static int manhattan_distance(Node a, Node b) {
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
		static int state_heuristic(State current, State target) {
			int heuristic = 0;

			for (int i = 0; i < current.items.Length && i < target.items.Length; i++) {
				heuristic += manhattan_distance(current.items[i], target.items[i]);
			}

			return heuristic;
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

		public static List<State> a_star(ref Grid grid, State start_state, State target_state) {
			SimplePriorityQueue<State, int> open_list = new SimplePriorityQueue<State, int>();
			HashSet<State> closed_list = new HashSet<State>();

			int cost;
			bool is_in_open;
			State current_state;
			List<State> neighbouring_states;

			/** For testing only. **/
			int iteration_count = 0;
			Stopwatch watch = new Stopwatch();
			/** For testing only. **/

			watch.Start();

			start_state.set_cost(0, state_heuristic(start_state, target_state));
			open_list.Enqueue(start_state, start_state.f_cost);

			while (open_list.Count > 0) {
				iteration_count++;

				current_state = open_list.Dequeue();
				closed_list.Add(current_state);

				if (current_state.Equals(target_state)) {
					watch.Stop();

					Console.WriteLine("\n\n==============================================\n");

					Console.WriteLine("END state:");
					grid.draw_grid(current_state);

					Console.WriteLine("\nFinished in: {0} iterations | {1} ms", iteration_count, watch.ElapsedMilliseconds);
					return reconstruct_path(current_state, start_state, target_state);
				}
				
				neighbouring_states = grid.expand_state(current_state, target_state);

				for (int i = 0; i < neighbouring_states.Count; i++) {
					if (!closed_list.Contains(neighbouring_states[i])) {
						cost = current_state.cost + state_heuristic(neighbouring_states[i], current_state);
						is_in_open = open_list.Contains(neighbouring_states[i]);

						if (cost < neighbouring_states[i].cost || !is_in_open) {
							neighbouring_states[i].set_cost(cost, state_heuristic(neighbouring_states[i], target_state));
							neighbouring_states[i].previous = current_state;

							if (!is_in_open) {
								open_list.Enqueue(neighbouring_states[i], neighbouring_states[i].f_cost);
							}
						}
					}
				}

				watch.Stop();
				Console.Write("\relapsed: {0} | iterations: {1} | heuristic: {2} | f-cost: {3}      ", watch.ElapsedMilliseconds, iteration_count, current_state.heuristic, current_state.f_cost);
				watch.Start();
			}

			watch.Stop();

			return null;
		}
	}
}
