﻿using System.Collections.Generic;

namespace atomixcs.a_star {
	class State {
		public List<Node> items;
		public int cost;
		public int heuristic;
		public int f_cost;
		public State previous;

		public State(List<Node> items) {
			this.items = items;
		}

		public void set_cost(int cost, int heuristic) {
			this.cost = cost;
			this.heuristic = heuristic;
			this.f_cost = this.cost + this.heuristic;
		}

		public override bool Equals(object obj) {
			if (obj == null) {
				return false;
			}

			State other = (State) obj;
			if ((System.Object) other == null) {
				return false;
			}

			if (this.items == null || this.items.Count <= 0) {
				if (other.items == null || this.items.Count <= 0) {
					return true;
				} else {
					return false;
				}
			}

			for (int i = 0; i < this.items.Count && i < other.items.Count; i++) {
				if (!this.items[i].Equals(other.items[i])) {
					return false;
				}
			}

			return true;
		}

		public override int GetHashCode() {
			int hash = 0;

			if (this.items != null && this.items.Count > 0) {
				for (int i = 0; i < this.items.Count; i++) {
					hash += this.items[i].GetHashCode() * 23;
				}
			} else {
				hash = base.GetHashCode();
			}

			return hash;
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
