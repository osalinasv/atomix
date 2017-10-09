using System;
using System.Collections.Generic;
using System.Drawing;

using atomixcs.a_star;

namespace atomixcs {
	struct XMLLevel {
		public string name;
		public string path;
		public Color[] colors;

		public XMLLevel(string name, string path, Color[] colors) {
			this.name = name;
			this.path = path;
			this.colors = colors;
		}
	}

	class Program {
		static void Main(string[] args) {
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			Console.WriteLine("A* Atomix\n");

			string root = AppContext.BaseDirectory;
			string data_dir = root + "data/";

			/**
			 * Serialize the data/data.xml and create a new XMLLevel object for each level tag, fill
			 * path with the string inside the path tag and create a color array based on each of the color tags inside
			 * the colors tag.
			 **/
			List<XMLLevel> levels = new List<XMLLevel>() {
				new XMLLevel("Level 01", "level-01/", new Color[] {
					Color.FromArgb(0, 0, 255),
					Color.FromArgb(0, 255, 255),
					Color.FromArgb(255, 0, 0),
				}),
				new XMLLevel("Level 02", "level-02/", new Color[] {
					Color.FromArgb(0, 0, 255),
					Color.FromArgb(0, 255, 255),
					Color.FromArgb(255, 0, 0),
					Color.FromArgb(255, 255, 0),
					Color.FromArgb(0, 255, 0),
				}),
			};

			int selected_level = 0;

			Console.WriteLine("Level selection.");

			for (int i = 0; i < levels.Count; i++) {
				Console.WriteLine("{0}. {1}", i + 1, levels[i].name);
			}

			Console.Write("\nSelect level [1-{0}]: ", levels.Count);
			selected_level = int.Parse(Console.ReadLine()) - 1;
			Console.WriteLine();

			string level_path = data_dir + levels[selected_level].path;
			Color[] level_colors = levels[selected_level].colors;

			Grid grid = grid_from_images(level_path + "diagram.png", level_path + "solution.png", level_colors);

			Console.WriteLine("START state:");
			grid.draw_grid(grid.start_state);
			Console.WriteLine();

			Console.WriteLine("TARGET state:");
			grid.draw_grid(grid.target_state);
			Console.WriteLine();

			List<State> path = AStar.a_star(ref grid, grid.start_state, grid.target_state);

			if (path != null && path.Count > 0) {
				Console.ReadLine();
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
	}
}
