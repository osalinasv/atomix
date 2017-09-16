using System;
using System.Collections.Generic;

namespace atomixcs.a_star {
	class Grid {
		public int width = 0;
		public int height = 0;

		public Node[,] nodes;
		public List<Vector2> walls;

		public List<Vector2> directions;

		public Grid(int width, int height) {
			this.width = width;
			this.height = height;

			this.directions = new List<Vector2> {
				new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(0, 1)
			};

			this.walls = new List<Vector2>();
		}

		public Grid(int width, int height, List<Vector2> walls) {
			this.width = width;
			this.height = height;
			this.walls = walls;

			this.directions = new List<Vector2> {
				new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(0, 1)
			};

			this.init_grid();
		}

		public void init_grid() {
			this.nodes = new Node[this.width, this.height];

			bool is_walkable = true;
			Vector2 position;

			for (int y = 0; y < this.height; y++) {
				for (int x = 0; x < this.width; x++) {
					position = new Vector2(x, y);

					if (this.walls.Contains(position)) {
						is_walkable = false;
					} else {
						is_walkable = true;
					}

					this.nodes[x, y] = new Node(x, y, is_walkable);
				}
			}
		}

		public bool is_position_in_bounds(Vector2 position) {
			return 0 <= position.x && position.x < this.width && 0 <= position.y && position.y < this.height;
		}

		public bool is_node_walkable(Node node) {
			return node.is_walkable;
		}

		public Node get_node_from_position(Vector2 position) {
			return this.nodes[position.x, position.y];
		}

		public List<Node> get_nodes_from_positions(List<Vector2> positions) {
			List<Node> nodes = new List<Node>();

			for (int i = 0; i < positions.Count; i++) {
				nodes.Add(get_node_from_position(positions[i]));
			}

			return nodes;
		}

		public Node get_closest_neighbour(Node node, Vector2 direction) {
			Vector2 position = node.position + direction;

			if (!this.is_position_in_bounds(position)) {
				return null;
			}

			Node neighbour = null;
			Node next_neighbour = this.nodes[position.x, position.y];

			while (this.is_position_in_bounds(position) && this.is_node_walkable(next_neighbour)) {
				neighbour = next_neighbour;
				position += direction;

				next_neighbour = this.nodes[position.x, position.y];
			}

			return neighbour;
		}

		public List<Node> get_neighbours(Node node) {
			Node neighbour = null;
			List<Node> neighbours = new List<Node>();

			for (int i = 0; i < directions.Count; i++) {
				neighbour = this.get_closest_neighbour(node, directions[i]);

				if (neighbour != null) {
					neighbours.Add(neighbour);
				}
			}

			return neighbours;
		}

		// @Important: added a target filter so the nodes that are already in their target position are not considered
		// in the search of neighbours, however there might be a case where a node is already in their target but needs
		// to move to serve as a stopper for another node.
		public List<State> expand_state(State current, State target = null) {
			List<State> neighbouring_states = new List<State>();
			List<Node> neighbours;

			List<Node> items;

			for (int i = 0; i < current.items.Count; i++) {
				if (target == null || current.items[i].position != target.items[i].position) {
					neighbours = get_neighbours(current.items[i]);

					for (int j = 0; j < neighbours.Count; j++) {
						if (!current.items.Contains(neighbours[j])) {
							items = new List<Node>(current.items);
							items[i] = neighbours[j];

							neighbouring_states.Add(new State(items));
						}
					}
				}
			}

			return neighbouring_states;
		}

		public void draw_grid(State current_state) {
			char character = ' ';
			int index = -1;
			Node node;

			List<Vector2> positions = new List<Vector2>();
			for (int i = 0; i < current_state.items.Count; i++) {
				positions.Add(current_state.items[i].position);
			}

			for (int y = 0; y < this.height; y++) {
				for (int x = 0; x < this.width; x++) {
					node = this.nodes[x, y];

					if (!node.is_walkable) {
						character = '\u25A0';
					} else {
						index = positions.IndexOf(node.position);

						if (index == 0) {
							character = 'C';
						} else if (index > 0) {
							character = 'H';
						} else {
							character = ' ';
						}
					}

					Console.Write("{0} ", character);
				}

				Console.WriteLine();
			}
		}
	}
}
