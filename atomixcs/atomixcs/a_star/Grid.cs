using System;
using System.Collections.Generic;

namespace atomixcs.a_star {
	class Grid {
		public int width = 0;
		public int height = 0;

		public Node[,] nodes;

		public List<Vector2> walls;
		public List<Vector2> start;
		public List<Vector2> target;

		public Grid(int width, int height) {
			this.width = width;
			this.height = height;

			this.walls = new List<Vector2>();
			this.start = new List<Vector2>();
			this.target = new List<Vector2>();
		}

		public Grid(int width, int height, List<Vector2> walls, List<Vector2> start, List<Vector2> target) {
			this.width = width;
			this.height = height;

			this.walls = walls;
			this.start = start;
			this.target = target;

			this.init_grid();
		}

		public void init_grid() {
			this.nodes = new Node[this.width, this.height];

			bool is_walkable = true;
			Vector2 position;

			for (int y = 0; y < this.height; y++) {
				for (int x = 0; x < this.width; x++) {
					position = new Vector2(x, y);

					if (this.walls.Contains(position) || this.start.Contains(position)) {
						is_walkable = false;
					} else {
						is_walkable = true;
					}

					this.nodes[x, y] = new Node(x, y, is_walkable);
				}
			}
		}

		public bool is_node_in_bounds(Node node) {
			return 0 <= node.position.x && node.position.x < this.width && 0 <= node.position.y && node.position.y < this.height;
		}

		public bool is_node_walkable(Node node) {
			return node.is_walkable;
		}

		public Node get_node_from_position(Vector2 position) {
			return this.nodes[position.x, position.y];
		}

		public Node get_closest_neighbour(Node node, Vector2 direction) {
			Vector2 position = node.position + direction;

			Node neighbour = null;
			Node next_neighbour = this.nodes[position.x, position.y];

			while (this.is_node_walkable(next_neighbour) && this.is_node_in_bounds(next_neighbour)) {
				neighbour = next_neighbour;
				position += direction;

				next_neighbour = this.nodes[position.x, position.y];
			}

			return neighbour;
		}

		public List<Node> get_neighbours(Node node) {
			Node neighbour = null;
			List<Node> neighbours = new List<Node>();
			List<Vector2> directions = new List<Vector2> {
				new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(0, 1)
			};

			foreach (Vector2 direction in directions) {
				neighbour = this.get_closest_neighbour(node, direction);

				if (neighbour != null) {
					neighbours.Add(neighbour);
				}
			}

			return neighbours;
		}

		public void draw_grid() {
			char character = ' ';
			int index = -1;
			Node node;

			for (int y = 0; y < this.height; y++) {
				for (int x = 0; x < this.width; x++) {
					node = this.nodes[x, y];

					if (!node.is_walkable) {
						character = '\u25A0';
						index = this.start.IndexOf(node.position);

						if (index == 0) {
							character = 'C';
						} else if (index > 0) {
							character = 'H';
						}
					} else {
						character = ' ';
					}

					Console.Write("{0} ", character);
				}

				Console.WriteLine();
			}
		}
	}
}
