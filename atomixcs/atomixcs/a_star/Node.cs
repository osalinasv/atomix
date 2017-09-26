namespace atomixcs.a_star {
	class Node {
		public Vector2 position;
		public bool is_walkable;

		public Node(int x, int y, bool is_walkable) {
			this.position = new Vector2(x, y);
			this.is_walkable = is_walkable;
		}

		public override bool Equals(object obj) {
			if (obj == null) {
				return false;
			}

			Node other = (Node) obj;
			if ((System.Object) other == null) {
				return false;
			}

			return this.position == other.position;
		}

		public override int GetHashCode() {
			return this.position.GetHashCode() * 23;
		}

		public override string ToString() {
			return "N" + this.position;
		}
	}

	// This class will not be needed once we start working on Unity.
	struct Vector2 {
		public int x;
		public int y;

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
			return this.x * 23 + this.y * 23;
		}

		public override string ToString() {
			return "<" + this.x + "," + this.y + ">";
		}
	}
}
