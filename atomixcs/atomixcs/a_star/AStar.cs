using System;
using System.Collections.Generic;

namespace atomixcs.a_star {
	class AStar {
		static float manhattan_heuristic(Node a, Node b) {
			return Math.Abs(a.position.x - b.position.x) + Math.Abs(a.position.y - b.position.y);
		}

		static List<Node> retrace_path(Node start, Node target) {
			List<Node> path = new List<Node>();
			Node current = target;

			while (current != start) {
				if (current.parent != null) {
					path.Add(current);
					current = current.parent;
				} else {
					break;
				}
			}

			if (path.Count > 0) {
				path.Reverse();
			}

			return path;
		}

		// @Important: replace this with proper priority queue se we can pop the lowest cost item.
		static Node get_lowest_cost(List<Node> list) {
			Node node = list[0];

			for (int i = 1; i < list.Count; i++) {
				if (list[i].f_cost < node.f_cost) {
					node = list[i];
				}
			}

			return node;
		}

		// @Refactor: start and target should now be of type List<Node>
		// maybe the start, target and current_state Lists should be List<List<Node>>
		// where each item of the List is another List representing a snapshot (state) of the atom nodes.
		public static List<Node> a_star(Grid grid, Node start, Node target) {
			List<Node> open_list = new List<Node>();
			List<Node> closed_list = new List<Node>();

			// @Refactor: add a current_state Node List to keep track of the current position of all atoms.
			// At the start, current_state will be equal to the start List and will be the only List that will change over time.

			// Each time a neighbour is selected the previous atom Node in the current_state is replaced.
			// If target and current_state Lists are exactly equal then all atoms have reached their goal.
			// --OR--
			// Each time a node from current_state reaches its pair at target (hence it has reached its goal),
			// remove that node from current_state, when current_state is empty the solution has been reached.

			Node current_node = null;
			List<Node> neighbours = null;

			float cost = 0;
			float heuristic = 0;
			bool is_in_open = false;

			start.set_cost(0, manhattan_heuristic(start, target));
			open_list.Add(start);

			while (open_list.Count > 0) {
				current_node = get_lowest_cost(open_list);
				open_list.Remove(current_node);
				closed_list.Add(current_node);

				if (current_node == target) {
					return retrace_path(start, target);
				}

				// @Refactor: because now we have multiple start points we must iterate over the current_state List and join
				// the neighbours of all the atoms in a single List.

				// Once the single List of neighbours is obtained, it is filtered with the nodes in the current_state List,
				// this way no atoms overlap each other at any moment.

				neighbours = grid.get_neighbours(current_node);

				foreach (Node neighbour in neighbours) {
					if (closed_list.Contains(neighbour)) {
						continue;
					}

					cost = current_node.cost + manhattan_heuristic(current_node, neighbour);
					is_in_open = open_list.Contains(neighbour);

					if (cost < neighbour.cost || !is_in_open) {
						// @Refactor: with multiple start point the heuristic is now the sum of the manhattan distances between
						// all current_state nodes and target nodes (this is why the order of the items is important, so
						// current_state[0] and target[0] reference the same atom type, for example the Carbon atom).

						heuristic = manhattan_heuristic(neighbour, target);
						neighbour.set_cost(cost, heuristic);
						neighbour.parent = current_node;

						if (!is_in_open) {
							open_list.Add(neighbour);
							// Somewhere arround here is probably where we would update current_state with the atom nodes that moved.
							// but that means we need a way of knowing which atom moved, from what node and to what node.
						}
					}
				}

				Console.WriteLine("\nCurrent path:");
				Node.print_list(retrace_path(start, current_node));
				Console.WriteLine();

				// Console.ReadLine();
			}

			return retrace_path(start, target);
		}
	}
}
