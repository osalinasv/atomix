using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atomixcs.a_star {
	public struct Vector2 {
		int x;
		int y;

		public Vector2(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public static Vector2 operator +(Vector2 a, Vector2 b) {
			return new Vector2(a.x + b.x, a.y + b.y);
		}

		public static Vector2 operator -(Vector2 a, Vector2 b) {
			return new Vector2(a.x - b.x, a.y - b.y);
		}

		public static bool operator ==(Vector2 a, Vector2 b) {
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Vector2 a, Vector2 b) {
			return !(a == b);
		}

		public override bool Equals(object obj) {
			if (obj == null) {
				return false;
			}

			Vector2 other = (Vector2) obj;
			if ((System.Object) other == null) {
				return false;
			}

			return this.x == other.x && this.y == other.y;
		}

		public override int GetHashCode() {
			return this.x ^ this.y;
		}
	}

	class Node {
		public Vector2 position = new Vector2(0, 0);
		public float cost = 0;
		public float heuristic = 0;
		public float f_cost = 0;

		public Node parent = null;
		public bool is_walkable = true;

		public Node(int x, int y, bool is_walkable) {
			this.position = new Vector2(x, y);
			this.is_walkable = is_walkable;
		}

		public float set_cost(float cost, float heuristic) {
			this.cost = cost;
			this.heuristic = heuristic;
			this.f_cost = this.cost + this.heuristic;

			return this.f_cost;
		}
	}
}
