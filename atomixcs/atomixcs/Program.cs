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
			Console.CursorVisible = false;

			string data_dir = AppContext.BaseDirectory + "data/";

			/** Read and deserialize level XML data **/
			List<XMLLevel> levels = read_level_data(data_dir + "data.xml");

			if (levels == null || levels.Count <= 0) {
				Console.WriteLine("No levels were found or there is a problem with data.xml");
				Console.ReadLine();
				return;
			}

			/** Level selection loop **/
			int selected_level;
			string level_path;
			Color[] level_colors;
			Grid grid;

			while (true) {
				selected_level = 0;
				display_menu(ref levels, ref selected_level);

				if (selected_level < 0 || selected_level >= levels.Count) {
					return;
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
					display_path(ref grid, ref path);
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

		static void display_menu(ref List<XMLLevel> levels, ref int selected_level) {
			int console_line_start;
			int console_char_start;
			bool enter_pressed = false;
			ConsoleKey key_pressed;

			Console.Clear();
			console_line_start = Console.CursorTop;
			console_char_start = Console.CursorLeft;

			while (!enter_pressed) {
				Console.CursorTop = console_line_start;
				Console.CursorLeft = console_char_start;
				Console.ResetColor();

				Console.WriteLine("   __           __    ____  _____  __  __  ____  _  _ \n  /__\\  \\|/    /__\\  (_  _)(  _  )(  \\/  )(_  _)( \\/ )\n /(__)\\ /|\\   /(__)\\   )(   )(_)(  )    (  _)(_  )  ( \n(__)(__)     (__)(__) (__) (_____)(_/\\/\\_)(____)(_/\\_)\n");
				Console.WriteLine("Omar Adrian Salinas Villanueva \nJesus Moya Lozano \nPablo Antonio García Vásquez \nLeonardo Alejandro Villanueva Betancour \n");
				Console.WriteLine("LEVEL SELECTION.\n");

				for (int i = 0; i <= levels.Count; i++) {
					if (i == selected_level) {
						Console.ForegroundColor = ConsoleColor.Green;
						Console.Write(" \u00BB ");
					} else {
						Console.Write(" \u00B7 ");
					}

					if (i < levels.Count) {
						Console.WriteLine("{0}", levels[i].name);
					} else {
						Console.WriteLine("Exit");
					}

					Console.ResetColor();
				}

				Console.WriteLine("\nPress \u2191 or \u2193 arrows to navigate...");
				Console.WriteLine("Press Enter to select.");

				if (Console.KeyAvailable) {
					key_pressed = Console.ReadKey(false).Key;

					if (key_pressed == ConsoleKey.Enter) {
						return;
					} else if (key_pressed == ConsoleKey.UpArrow) {
						selected_level--;
					} else if (key_pressed == ConsoleKey.DownArrow) {
						selected_level++;
					}

					selected_level = Math.Min(Math.Max(0, selected_level), levels.Count);
				}
			}
		}

		static void display_path(ref Grid grid, ref List<State> path) {
			int console_line_start;

			if (path != null && path.Count > 0) {
				Console.WriteLine("\n\nPATH:");
				Console.WriteLine("Solved in " + (path.Count - 1) + " steps.\n");

				console_line_start = Console.CursorTop;

				int i = 0;
				ConsoleKey key_pressed;

				while (i >= 0 && i < path.Count) {
					Console.CursorTop = console_line_start;

					grid.draw_grid(path[i]);
					Console.WriteLine("{0}. {1}      ", (i <= 0) ? "start" : i.ToString(), path[i]);
					Console.WriteLine();

					Console.WriteLine("Press \u2190 or \u2192 arrows to navigate...");
					if (i >= path.Count) {
						break;
					}

					if (Console.KeyAvailable) {
						key_pressed = Console.ReadKey(false).Key;

						if (key_pressed == ConsoleKey.LeftArrow) {
							i--;
						} else if (key_pressed == ConsoleKey.RightArrow) {
							i++;
						}

						i = Math.Min(Math.Max(0, i), path.Count);
					}
				}
			} else {
				Console.WriteLine("No solution found");
				Console.WriteLine("Press Enter to return to menu");
				Console.ReadLine();
			}
		}
	}
}
