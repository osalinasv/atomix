using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using atomixcs.a_star;

namespace atomixcs {
	class Program {
		static List<Vector2> get_walls_from_image(Bitmap image) {
			List<Vector2> pixels = new List<Vector2>();
			Color wall_color = Color.FromArgb(0, 0, 0);

			int width = image.Width;
			int height = image.Height;

			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (wall_color.Equals(image.GetPixel(x, y))) {
						pixels.Add(new Vector2(x, y));
					}
				}
			}

			return pixels;
		}

		static List<Vector2> get_atoms_from_image(Bitmap image) {
			Vector2[] atoms = new Vector2[5];
			Color carbon_color = Color.FromArgb(0, 0, 255);
			Color hidrogen_left = Color.FromArgb(255, 255, 0);
			Color hidrogen_top = Color.FromArgb(255, 0, 0);
			Color hidrogen_right = Color.FromArgb(0, 255, 255);
			Color hidrogen_bottom = Color.FromArgb(0, 255, 0);

			int width = image.Width;
			int height = image.Height;

			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					Color color = image.GetPixel(x, y);

					if (carbon_color.Equals(color)) {
						atoms[0] = new Vector2(x, y);
					} else if (hidrogen_right.Equals(color)) {
						atoms[1] = new Vector2(x, y);
					} else if (hidrogen_top.Equals(color)) {
						atoms[2] = new Vector2(x, y);
					} else if (hidrogen_left.Equals(color)) {
						atoms[3] = new Vector2(x, y);
					} else if (hidrogen_bottom.Equals(color)) {
						atoms[4] = new Vector2(x, y);
					}
				}
			}

			return atoms.ToList();
		}

		static void Main(string[] args) {
			Console.WriteLine("A* Atomix\n\n");

			string root = AppDomain.CurrentDomain.BaseDirectory;

			Bitmap diagram = new Bitmap(root + "data/diagram.png");
			Bitmap solution = new Bitmap(root + "data/solution.png");

			int width = diagram.Width;
			int height = diagram.Height;

			List<Vector2> walls = get_walls_from_image(diagram);
			List<Vector2> start = get_atoms_from_image(diagram);
			List<Vector2> target = get_atoms_from_image(solution);

			Grid grid = new Grid(width, height, walls);
			grid.draw_grid(start);
			grid.draw_grid(target);

			List<Node> path = AStar.a_star(grid, grid.get_node_from_position(start[0]), grid.get_node_from_position(target[0]));
			Node.print_list(path);

			Console.ReadLine();
		}
	}
}
