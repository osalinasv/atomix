using System;
using System.Collections.Generic;

namespace atomixcs.a_star {
	class Grid {
		private int width;
		private int height;
		private Node[,] nodes;
		public State[] states;
		public List<Vector2> walls;
		private List<Vector2> directions;

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

			int x, y;

			for (y = 0; y < this.height; y++) {
				for (x = 0; x < this.width; x++) {
					position = new Vector2(x, y);
					is_walkable = !this.walls.Contains(position);

					this.nodes[x, y] = new Node(x, y, is_walkable);
				}
			}
		}

		public bool is_position_in_bounds(Vector2 position) {
			return 0 <= position.x && position.x < this.width && 0 <= position.y && position.y < this.height;
		}

		public bool is_node_walkable(Node node) {
			return node != null && node.is_walkable;
		}

		public Node get_node_from_position(Vector2 position) {
			if (this.is_position_in_bounds(position)) {
				return this.nodes[position.x, position.y];
			} else {
				return null;
			}
		}

		public List<Node> get_nodes_from_positions(List<Vector2> positions) {
			List<Node> nodes = new List<Node>();

			for (int i = 0; i < positions.Count; i++) {
				nodes.Add(get_node_from_position(positions[i]));
			}

			return nodes;
		}

		public Node get_neighbour_in_direction(Node node, State current, Vector2 direction) {
			Vector2 position = node.position + direction;

			if (!this.is_position_in_bounds(position)) {
				return null;
			}

			Node neighbour = null;
			Node next_neighbour = this.nodes[position.x, position.y];

			while (this.is_position_in_bounds(position) && this.is_node_walkable(next_neighbour) && !current.items.Contains(next_neighbour)) {
				neighbour = next_neighbour;
				next_neighbour = this.nodes[position.x, position.y];

				position += direction;
			}

			return neighbour;
		}

		public List<Node> get_neighbours(Node node, State current) {
			List<Node> neighbours = new List<Node>();
			Node neighbour = null;

			for (int i = 0; i < this.directions.Count; i++) {
				neighbour = this.get_neighbour_in_direction(node, current, directions[i]);

				if (neighbour != null) {
					neighbours.Add(neighbour);
				}
			}

			return neighbours;
		}

		public List<State> expand_state(State current) {
			List<State> neighbouring_states = new List<State>();
			List<Node> neighbours;
			List<Node> items;

			int i, j;

			for (i = 0; i < current.items.Count; i++) {
				neighbours = get_neighbours(current.items[i], current);

				for (j = 0; j < neighbours.Count; j++) {
					items = new List<Node>(current.items);
					items[i] = neighbours[j];

					neighbouring_states.Add(new State(items));
				}
			}

			return neighbouring_states;
		}

		public void draw_grid(State current_state) {
			char character = ' ';
			int index = -1;
			Node node;

			int i, x, y;

			List<Vector2> positions = new List<Vector2>();
			for (i = 0; i < current_state.items.Count; i++) {
				positions.Add(current_state.items[i].position);
			}

			for (y = 0; y < this.height; y++) {
				for (x = 0; x < this.width; x++) {
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
