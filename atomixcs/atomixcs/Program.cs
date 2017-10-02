using System;
using System.Collections.Generic;
using System.Drawing;

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
		
		static Vector2[] get_atoms_from_image(Bitmap image, Color[] pixels) {
			Vector2[] atoms = new Vector2[pixels.Length];

			int width = image.Width;
			int height = image.Height;
			int index;

			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					Color color = image.GetPixel(x, y);

					index = Array.IndexOf(pixels, color);

					if (index >= 0 && index < atoms.Length) {
						atoms[index] = new Vector2(x, y);
					}
				}
			}

			return atoms;
		}

		static Grid grid_from_images(string diagram_path, string solution_path, Color[] pixels) {
			Bitmap diagram = new Bitmap(diagram_path);
			Bitmap solution = new Bitmap(solution_path);

			int width = diagram.Width;
			int height = diagram.Height;

			Vector2[] wall_positions = get_walls_from_image(diagram);
			Vector2[] start_positions = get_atoms_from_image(diagram, pixels);
			Vector2[] target_positions = get_atoms_from_image(solution, pixels);

			return new Grid(width, height, wall_positions, start_positions, target_positions);
		}

		static void Main(string[] args) {
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			Console.WriteLine("A* Atomix\n");

			string root = AppDomain.CurrentDomain.BaseDirectory;

			Color[] pixels_level_01 = new Color[] {
				Color.FromArgb(0, 0, 255),
				Color.FromArgb(0, 255, 255),
				Color.FromArgb(255, 0, 0),
			};

			Color[] pixels_level_02 = new Color[] {
				Color.FromArgb(0, 0, 255),
				Color.FromArgb(0, 255, 255),
				Color.FromArgb(255, 0, 0),
				Color.FromArgb(255, 255, 0),
				Color.FromArgb(0, 255, 0),
			};

			Grid grid = grid_from_images(root + "data/diagram.png", root + "data/solution.png", pixels_level_01);

			Console.WriteLine("START state:");
			grid.draw_grid(grid.start_state);
			Console.WriteLine(grid.start_state);
			Console.WriteLine();

			Console.WriteLine("TARGET state:");
			grid.draw_grid(grid.target_state);
			Console.WriteLine(grid.target_state);

			Console.WriteLine("\n==============================================\n");

			List<State> path = AStar.a_star(grid, grid.start_state, grid.target_state);

			Console.ReadLine();

			Console.WriteLine("==============================================\n");

			if (path != null && path.Count > 0) {
				Console.WriteLine("PATH:\n");

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
