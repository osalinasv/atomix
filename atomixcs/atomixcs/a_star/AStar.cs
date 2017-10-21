using System;
using System.Collections.Generic;
using System.Diagnostics;

using Priority_Queue;

namespace atomixcs.a_star {
	class AStar {
		static int manhattan_distance(Node a, Node b) {
			return Math.Abs(a.position.x - b.position.x) + Math.Abs(a.position.y - b.position.y);
		}

		static int state_heuristic(State current, State target) {
			int heuristic = 0;
			Node first_current = current.items[0];
			Node first_target = target.items[0];

			Vector2 distance_current;
			Vector2 distance_target;
			Vector2 difference;

			heuristic += manhattan_distance(first_current, first_target);

			for (int i = 1; i < current.items.Length && i < target.items.Length; i++) {
				distance_current = first_current.position - current.items[i].position;
				distance_target = first_target.position - target.items[i].position;

				difference = distance_target - distance_current;
				heuristic += Math.Abs(difference.x) + Math.Abs(difference.y);
			}

			return heuristic;
		}

		static bool compare_to_target(State current, State target) {
			Node first_current = current.items[0];
			Node first_target = target.items[0];

			Vector2 distance_current;
			Vector2 distance_target;

			for (int i = 1; i < current.items.Length && i < target.items.Length; i++) {
				distance_current = first_current.position - current.items[i].position;
				distance_target = first_target.position - target.items[i].position;

				if (distance_current != distance_target) {
					return false;
				}
			}

			return true;
		}

		static List<State> reconstruct_path(State current_state, State start_state, State target_state) {
			List<State> path = new List<State>();
			State current = current_state;

			if (compare_to_target(current_state, target_state)) {
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

		static string format_time(float milliseconds) {
			int y = 60 * 60 * 1000;
			int h = (int) (milliseconds / y);
			int m = (int) ((milliseconds - (h * y)) / (y / 60));
			int s = (int) ((milliseconds - (h * y) - (m * (y / 60))) / 1000);
			int mi = (int) (milliseconds - (h * y) - (m * (y / 60)) - (s * 1000));

			return h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00") + ":" + mi.ToString("000");
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

				if (compare_to_target(current_state, target_state)) {
					watch.Stop();

					Console.WriteLine("\n\n==============================================\n");

					Console.WriteLine("END state:");
					grid.draw_grid(current_state);

					Console.WriteLine("\nFinished in: {0} iterations | {1}", iteration_count, format_time(watch.ElapsedMilliseconds));
					return reconstruct_path(current_state, start_state, target_state);
				}
				
				neighbouring_states = grid.expand_state(current_state);

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

				Console.Write("\relapsed: {0} | iterations: {1} | heuristic: {2} | f-cost: {3}      ", format_time(watch.ElapsedMilliseconds), iteration_count, current_state.heuristic, current_state.f_cost);
				watch.Start();
			}

			watch.Stop();

			return null;
		}
	}
}
