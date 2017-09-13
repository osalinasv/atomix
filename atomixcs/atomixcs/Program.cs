using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		static void Main(string[] args) {
			Console.WriteLine("A* Atomix\n\n");

			string root = AppDomain.CurrentDomain.BaseDirectory;

			Bitmap diagram = new Bitmap(root + "data/diagram.png");
			Bitmap solution = new Bitmap(root + "data/solution.png");

			int width = diagram.Width;
			int height = diagram.Height;

			List<Vector2> walls = get_walls_from_image(diagram);

			List<Vector2> start = new List<Vector2> {
				new Vector2(1, 1), new Vector2(2, 1)
			};

			List<Vector2> target = new List<Vector2> {
				new Vector2(2, 1)
			};

			Grid grid = new Grid(width, height, walls, start, target);
			grid.draw_grid();

			Console.ReadLine();
		}
	}
}
