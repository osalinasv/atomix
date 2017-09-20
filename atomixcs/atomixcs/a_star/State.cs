using System.Collections.Generic;

namespace atomixcs.a_star {
	class State {
		public List<Node> items;
		public float cost;
		public float heuristic;
		public float f_cost;

		public State(List<Node> items) {
			this.items = items;
		}

		public void set_cost(float cost, float heuristic) {
			this.cost = cost;
			this.heuristic = heuristic;
			this.f_cost = this.cost + this.heuristic;
		}

		public override string ToString() {
			string str = "S[";

			for (int i = 0; i < this.items.Count; i++) {
				str += this.items[i].ToString();

				if (i < this.items.Count - 1) {
					str += ", ";
				}
			}

			str += "] h=" + this.heuristic + " f=" + this.f_cost;

			return str;
		}
	}
}
