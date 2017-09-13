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

			return path;
		}

		// @Important: replace this with proper priority queue.
		static Node get_lowest_cost(List<Node> list) {
			Node node = list[0];

			for (int i = 1; i < list.Count; i++) {
				if (list[i].f_cost < node.f_cost) {
					node = list[i];
				}
			}

			return node;
		}

		public static List<Node> a_star(Grid grid, Node start, Node target) {
			List<Node> open_list = new List<Node>();
			List<Node> closed_list = new List<Node>();

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

				Console.Write("current_node: {0}", current_node);

				if (current_node == target) {
					return retrace_path(start, target);
				}

				neighbours = grid.get_neighbours(current_node);

				Console.WriteLine("\nneighbours:");
				Node.print_list(neighbours);
				Console.WriteLine();

				foreach (Node neighbour in neighbours) {
					if (closed_list.Contains(neighbour)) {
						Console.WriteLine("Neighbour was in closed list");
						continue;
					}

					cost = current_node.cost + manhattan_heuristic(current_node, neighbour);
					is_in_open = open_list.Contains(neighbour);

					if (cost < neighbour.cost || !is_in_open) {
						heuristic = manhattan_heuristic(neighbour, target);
						neighbour.set_cost(cost, heuristic);
						neighbour.parent = current_node;

						if (!is_in_open) {
							open_list.Add(neighbour);
							Console.WriteLine("neighbour: {0}\t f_cost: {1}", neighbour, neighbour.f_cost);

							Console.WriteLine("\nopen_list:");
							Node.print_list(open_list);
							Console.WriteLine();
						}
					} else {
						Console.WriteLine("New cost is bigger or neighbour already in open_list");
					}
				}

				Console.WriteLine("\nCurrent path:");
				Node.print_list(retrace_path(start, current_node));
				Console.WriteLine();

				Console.ReadLine();
			}

			return retrace_path(start, target);
		}
	}
}
