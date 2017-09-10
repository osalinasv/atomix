from a_star import Node
from a_star import NodeType


class Diagram:
	def __init__(self, width: int, height: int, walls = [], start = [], end = []):
		self.width = width
		self.height = height

		self.nodes : [] = []
		self.walls = walls

		self.start = start
		self.end = end

	def initNodes(self):
		for y in range(0, self.height):
			self.nodes.append([])
			for x in range(0, self.width):
				type : NodeType = NodeType.DEFAULT
				position = (x, y)

				if position in self.walls:
					type = NodeType.WALL
				elif position in self.start:
					type = NodeType(NodeType.CARBON - self.start.index(position))

				self.nodes[y].append(Node(x, y, type))

	def drawDiagram(self, width = 2):
		print("%%-%ds" % width % " ", end = "")
		for x in range(0, self.width):
			print("%%-%ds" % width % x, end = "")
		print()

		for y in range(0, self.height):
			print("%%-%ds" % width % y, end = "")
			for x in range(0, self.width):
				node : Node = self.nodes[y][x]
				character : str = ' '

				if NodeType.DEFAULT < node.type < NodeType.CARBON:
					character = 'H'
				elif node.type == NodeType.CARBON:
					character = 'C'
				elif node.type == NodeType.WALL:
					character = '\u25A0'

				print("%%-%ds" % width % character, end = "")
			print()

	def isInBounds(self, node : Node):
		return 0 <= node.position[0] < self.width and 0 <= node.position[1] < self.height

	def isPassable(self, node : Node):
		return node.type == NodeType.DEFAULT

	def getNode(self, position : ()):
		return self.nodes[position[1]][position[0]]

	def getNodePosition(self, node : Node):
		for y, list in enumerate(self.nodes):
			for x, n in enumerate(list):
				if n == node and node in list:
					return (x, y)

	def getNeighbor(self, position: (), direction: ()):
		pos = (position[0] + direction[0], position[1] + direction[1])

		neighbor : Node = None
		nextNeighbor : Node = self.getNode(pos)

		while self.isPassable(nextNeighbor) and self.isInBounds(nextNeighbor):
			neighbor = nextNeighbor

			pos = (pos[0] + direction[0], pos[1] + direction[1])
			nextNeighbor: Node = self.getNode(pos)

		return neighbor

	def getNeighbors(self, node : Node):
		directions = [(1, 0), (0, -1), (-1, 0), (0, 1)]
		position = self.getNodePosition(node)
		neighbors = []

		for direction in directions:
			neighbor = self.getNeighbor(position, direction)

			if (neighbor):
				neighbors.append(neighbor)

		return neighbors