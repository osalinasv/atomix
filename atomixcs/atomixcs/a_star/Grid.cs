using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atomixcs.a_star {
	class Grid {
		public int width = 0;
		public int height = 0;

		public Node[,] nodes;

		public List<Vector2> walls = new List<Vector2>();
		public List<Vector2> start = new List<Vector2>();
		public List<Vector2> target = new List<Vector2>();

		public Grid(int width, int height) {
			this.width = width;
			this.height = height;
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
