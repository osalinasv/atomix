from a_star import NodeType


class Node:
	def __init__(self, x : int, y : int, type : NodeType):
		self.position : () = (x, y)
		self.cost : float
		self.heuristic : float
		self.f : float

		self.parent : Node = None
		self.type : NodeType = type

	def __str__(self):
		return str(self.position)

	def setCost(self, g : float, h : float):
		self.cost = g
		self.heuristic = h
		self.f = self.cost + self.heuristic
