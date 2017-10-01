using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using atomixcs.a_star;

namespace atomixcs {
	class Program {
		static Vector2[] get_walls_from_image(Bitmap image) {
			List<Vector2> positions = new List<Vector2>();
			Color wall_color = Color.FromArgb(0, 0, 0);

			int width = image.Width;
			int height = image.Height;

			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (wall_color.Equals(image.GetPixel(x, y))) {
						positions.Add(new Vector2(x, y));
					}
				}
			}

			return positions.ToArray();
		}

		/**
		 * @Robustness: change the way atoms are found to support a variable amount of atoms and not just five.
		 * Whould need to somehow detect the number of diferent colored pixels, or pass a list of expected colors
		 * in a preestablished order.
		 **/
		static Vector2[] get_atoms_from_image(Bitmap image, List<Color> pixels) {
			Vector2[] atoms = new Vector2[pixels.Count];

			int width = image.Width;
			int height = image.Height;
			int index;

			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					Color color = image.GetPixel(x, y);

					index = pixels.IndexOf(color);

					if (index >= 0 && index < atoms.Length) {
						atoms[index] = new Vector2(x, y);
					}
				}
			}

			return atoms;
		}

		static void Main(string[] args) {
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			Console.WriteLine("A* Atomix\n");

			string root = AppDomain.CurrentDomain.BaseDirectory;

			Bitmap diagram = new Bitmap(root + "data/diagram.png");
			Bitmap solution = new Bitmap(root + "data/solution.png");

			int width = diagram.Width;
			int height = diagram.Height;

			List<Color> pixels_level_01 = new List<Color> {
				Color.FromArgb(0, 0, 255),
				Color.FromArgb(0, 255, 255),
				Color.FromArgb(255, 0, 0),
			};

			List<Color> pixels_level_02 = new List<Color> {
				Color.FromArgb(0, 0, 255),
				Color.FromArgb(0, 255, 255),
				Color.FromArgb(255, 0, 0),
				Color.FromArgb(255, 255, 0),
				Color.FromArgb(0, 255, 0),
			};

			Vector2[] wall_positions = get_walls_from_image(diagram);
			Vector2[] start_positions = get_atoms_from_image(diagram, pixels_level_01);
			Vector2[] target_positions = get_atoms_from_image(solution, pixels_level_01);

			Grid grid = new Grid(width, height, wall_positions);

			State start_state = new State(grid.get_nodes_from_positions(start_positions));
			State target_state = new State(grid.get_nodes_from_positions(target_positions));

			Console.WriteLine("START state:");
			grid.draw_grid(start_state);
			Console.WriteLine(start_state);
			Console.WriteLine();

			Console.WriteLine("TARGET state:");
			grid.draw_grid(target_state);
			Console.WriteLine(target_state);

			Console.WriteLine("\n==============================================\n");

			List<State> path = AStar.a_star(grid, start_state, target_state);

			Console.ReadLine();

			Console.WriteLine("\n==============================================\n");

			if (path != null && path.Count > 0) {
				Console.WriteLine("PATH:");

				foreach (State state in path) {
					grid.draw_grid(state);
					Console.WriteLine(state);
					Console.WriteLine();
				}

				Console.WriteLine("Solved in: " + (path.Count - 1) + " steps.");
			} else {
				Console.WriteLine("No solution found");
			}

			Console.ReadLine();
		}
	}
}
