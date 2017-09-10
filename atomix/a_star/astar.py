from a_star import Diagram
from a_star import Node
from a_star import PriorityQueue

def manhattanHeuristic(a : Node, b: Node) -> float:
	return abs(a.position[0] - b.position[0]) + abs(a.position[1] - b.position[1])

def isPositionInList(node : Node, list : []) -> Node:
	for n in list:
		if node.position == n.position:
			return n
	return None

def reconstructPath(closedList : []) -> []:
	path = closedList
	return path

def a_star(diagram : Diagram, start : Node, end : Node) -> []:
	openList = PriorityQueue()
	closedList = []

	start.setCost(0, manhattanHeuristic(start, end))
	openList.put(start, start.f)

	while not openList.empty():
		currentNode = openList.get()
		print("current:", currentNode)

		for neighbor in diagram.getNeighbors(currentNode):
			neighbor.parent = currentNode

			if neighbor is end:
				return reconstructPath(closedList)

			cost = currentNode.cost + manhattanHeuristic(neighbor, currentNode)
			heuristic = manhattanHeuristic(end, neighbor)
			neighbor.setCost(cost, heuristic)

			inOpen = isPositionInList(neighbor, openList.elements)
			if inOpen and inOpen.f < neighbor.f:
				continue

			inClosed = isPositionInList(neighbor, closedList)
			if inClosed and inClosed.f < neighbor.f:
				continue

			print("n:", neighbor, "\t", "c:", neighbor.cost, "\t", "h:", neighbor.heuristic, "\t", "f:", neighbor.f)
			openList.put(neighbor, neighbor.f)
		print("end neighbors\n")

		closedList.append(currentNode)

	return reconstructPath(closedList)