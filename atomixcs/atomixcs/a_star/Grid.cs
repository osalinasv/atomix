using System;
using System.Collections.Generic;

namespace atomixcs.a_star {
	class Grid {
		private int width = 0;
		private int height = 0;
		private Node[,] nodes;
		public State start_state;
		public State target_state;
		private List<State> cached_states;

		private static readonly Vector2[] directions = new Vector2[] {
			new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(0, 1)
		};

		public Grid(int width, int height, Vector2[] walls, Vector2[] start_positions, Vector2[] target_positions) {
			this.width = width;
			this.height = height;

			this.init_grid(walls);

			this.start_state = new State(this.get_nodes_from_positions(start_positions));
			this.target_state = new State(this.get_nodes_from_positions(target_positions));
		}

		private void init_grid(Vector2[] walls) {
			this.nodes = new Node[this.width, this.height];
			this.cached_states = new List<State>();

			bool is_walkable = true;
			Vector2 position;

			int x, y;

			for (y = 0; y < this.height; y++) {
				for (x = 0; x < this.width; x++) {
					position = new Vector2(x, y);
					is_walkable = Array.IndexOf(walls, position) <= -1;

					this.nodes[x, y] = new Node(x, y, is_walkable);
				}
			}
		}

		private bool is_position_in_bounds(Vector2 position) {
			return 0 <= position.x && position.x < this.width && 0 <= position.y && position.y < this.height;
		}

		private bool is_node_walkable(Node node) {
			return node != null && node.is_walkable;
		}

		private Node get_node_from_position(Vector2 position) {
			if (this.is_position_in_bounds(position)) {
				return this.nodes[position.x, position.y];
			} else {
				return null;
			}
		}

		public Node[] get_nodes_from_positions(Vector2[] positions) {
			List<Node> nodes = new List<Node>();
			Node node;

			for (int i = 0; i < positions.Length; i++) {
				node = get_node_from_position(positions[i]);

				if (node != null) {
					nodes.Add(node);
				}
			}

			return nodes.ToArray();
		}

		private Node get_neighbour_in_direction(Node node, State current, Vector2 direction) {
			Vector2 position = node.position + direction;

			if (!this.is_position_in_bounds(position)) {
				return null;
			}

			Node neighbour = null;
			Node next_neighbour = this.nodes[position.x, position.y];

			while (this.is_position_in_bounds(position) && this.is_node_walkable(next_neighbour) && Array.IndexOf(current.items, next_neighbour) <= -1) {
				neighbour = next_neighbour;
				next_neighbour = this.nodes[position.x, position.y];

				position += direction;
			}

			return neighbour;
		}

		private List<Node> get_neighbours(Node node, State current) {
			List<Node> neighbours = new List<Node>();
			Node neighbour = null;

			for (int i = 0; i < directions.Length; i++) {
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

			State new_state = null;
			Node[] items;
			int i, j, index;

			for (i = 0; i < current.items.Length; i++) {
				neighbours = get_neighbours(current.items[i], current);

				for (j = 0; j < neighbours.Count; j++) {
					items = (Node[]) current.items.Clone();
					items[i] = neighbours[j];

					new_state = new State(items);
					index = -1;

					if (this.cached_states.Count > 0) {
						index = this.cached_states.IndexOf(new_state);

						if (index >= 0) {
							new_state = this.cached_states[index];
						}
					}

					if (index == -1) {
						this.cached_states.Add(new_state);
					}

					neighbouring_states.Add(new_state);
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
			for (i = 0; i < current_state.items.Length; i++) {
				positions.Add(current_state.items[i].position);
			}

			ConsoleColor[] colors = {
				ConsoleColor.Magenta,
				ConsoleColor.Red,
				ConsoleColor.Yellow,
				ConsoleColor.Green
			};

			for (y = 0; y < this.height; y++) {
				for (x = 0; x < this.width; x++) {
					node = this.nodes[x, y];

					if (!node.is_walkable) {
						character = '\u25A0';
					} else {
						index = positions.IndexOf(node.position);

						if (index == 0 || index > 0) {
							if (index == 0) {
								Console.ForegroundColor = ConsoleColor.Cyan;
								character = 'C';
							} else {
								if (index - 1 >= 0 && index - 1 < colors.Length) {
									Console.ForegroundColor = colors[index - 1];
								} else {
									Console.ForegroundColor = ConsoleColor.Green;
								}

								character = 'H';
							}
						} else {
							character = ' ';
						}
					}

					Console.Write("{0} ", character);
					Console.ResetColor();
				}

				Console.WriteLine();
			}
		}
	}
}
