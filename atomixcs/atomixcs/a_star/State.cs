using System;
using System.Collections.Generic;

namespace atomixcs.a_star {
	class State {
		public List<Node> items;
		public float cost = 0;
		public float heuristic = 0;
		public float f_cost = 0;

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

			str += "]";

			return str;
		}
	}
}
