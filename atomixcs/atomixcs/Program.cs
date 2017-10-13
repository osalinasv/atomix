using System;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;
using System.Collections.Generic;

using atomixcs.a_star;

namespace atomixcs {
	public struct XMLColor {
		[XmlIgnore]
		public Color color;
		[XmlAttribute("hex")]
		public string color_string {
			get {
				return this.color.ToString();
			}
			set {
				this.color = ColorTranslator.FromHtml(value);
			}
		}

		public static implicit operator Color(XMLColor color) {
			return color.color;
		}
	}

	public struct XMLLevel {
		[XmlElement(DataType = "string", ElementName = "name")]
		public string name;
		[XmlElement(DataType = "string", ElementName = "path")]
		public string path;
		[XmlArray("colors"), XmlArrayItem("color")]
		public XMLColor[] colors;

		public XMLLevel(string name, string path, XMLColor[] colors) {
			this.name = name;
			this.path = path;
			this.colors = colors;
		}

		public Color[] get_color_array() {
			return Array.ConvertAll<XMLColor, Color>(this.colors, (XMLColor color) => { return color.color; });
		}
	}

	[XmlRoot("levels")]
	public struct XMLLevelList {
		[XmlElement("level")]
		public List<XMLLevel> levels;
	}

	class Program {
		static void Main(string[] args) {
			/** General initializations **/
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			Console.WriteLine("A* Atomix\n");

			string root = AppContext.BaseDirectory;
			string data_dir = root + "data/";

			/** Read and deserialize level XML data **/
			List<XMLLevel> levels = read_level_data(data_dir + "data.xml");

			if (levels == null || levels.Count <= 0) {
				Console.WriteLine("No levels were found or there is a problem with data.xml");
				return;
			}

			/** Level selection loop **/
			int selected_level;
			int numeric;
			bool should_exit;
			bool is_numeric;
			string level_path;
			string input;
			Color[] level_colors;
			Grid grid;

			while (true) {
				Console.Clear();
				Console.WriteLine("Level selection.");

				for (int i = 0; i < levels.Count; i++) {
					Console.WriteLine("{0}. {1}", i + 1, levels[i].name);
				}

				Console.WriteLine("{0}. Exit", levels.Count + 1);

				Console.Write("\nSelect level [1-{0}] (or type exit): ", levels.Count);
				input = Console.ReadLine();
				Console.WriteLine();

				should_exit = false;

				if (input.Equals("exit")) {
					should_exit = true;
				} else {
					is_numeric = int.TryParse(input, out numeric);

					if (!is_numeric) {
						Console.WriteLine("Invalid level input");
					} else {
						selected_level = int.Parse(input) - 1;

						if (selected_level < 0 || selected_level >= levels.Count) {
							if (selected_level == levels.Count) {
								should_exit = true;
							} else {
								Console.WriteLine("Invalid level number");
							}
						} else {
							/** Base grid and states creation **/
							level_path = data_dir + levels[selected_level].path;
							level_colors = levels[selected_level].get_color_array();

							Console.Clear();
							Console.WriteLine("{0}\n", levels[selected_level].name);
							grid = grid_from_images(level_path + "diagram.png", level_path + "solution.png", level_colors);

							Console.WriteLine("START state:");
							grid.draw_grid(grid.start_state);
							Console.WriteLine();

							Console.WriteLine("TARGET state:");
							grid.draw_grid(grid.target_state);
							Console.WriteLine();

							/** A* solution path search **/
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
					}
				}

				if (should_exit) {
					return;
				}
			}
		}

		static List<XMLLevel> read_level_data(string path) {
			XmlSerializer serializer = new XmlSerializer(typeof(XMLLevelList));
			List<XMLLevel> levels = null;

			using (StreamReader writter = new StreamReader(path)) {
				XMLLevelList level_list = (XMLLevelList)serializer.Deserialize(writter);
				levels = level_list.levels;
			}

			return levels;
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
