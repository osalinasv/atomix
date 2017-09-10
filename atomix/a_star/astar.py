from a_star import Diagram
from a_star import Node
from a_star import PriorityQueue

def manhattanHeuristic(a : Node, b: Node) -> float:
	return abs(a.position[0] - b.position[0]) + abs(a.position[1] - b.position[1])

def isPositionInOpen(node : Node, list : PriorityQueue) -> Node:
	inOpen = list.find(node)
	if inOpen and node == inOpen:
		return inOpen
	else:
		return None

def isPositionInClosed(node : Node, list : []) -> Node:
	for n in list:
		if node == n:
			return n
	return None

def reconstructPath(closedList : []) -> []:
	path = closedList
	return path

def a_star(diagram : Diagram, start : Node, end : Node) -> []:
	openList = PriorityQueue()
	closedList = []

	currentNode : Node = None
	neighbors : [Node] = None
	addToOpen = False

	start.setCost(0, manhattanHeuristic(start, end))
	openList.put(start, start.f)

	while not openList.isEmpty():
		currentNode = openList.pop()
		print("CURRENT:", currentNode)

		if currentNode == end:
			return reconstructPath(closedList)

		neighbors = diagram.getNeighbors(currentNode)
		print("NEIGHBORS for CURRENT:", neighbors, "\n")
		for neighbor in neighbors:
			neighbor.parent = currentNode

			cost = currentNode.cost + manhattanHeuristic(neighbor, currentNode)
			heuristic = manhattanHeuristic(end, neighbor)
			neighbor.setCost(cost, heuristic)

			print("n:", neighbor, "\t", "c:", neighbor.cost, "\t", "h:", neighbor.heuristic, "\t", "f:", neighbor.f)

			# @Important: [closed-list] This section is causing problems.
			addToOpen = True
			inOpen = isPositionInOpen(neighbor, openList)
			print("in open:", inOpen)
			if inOpen != None and inOpen.f < neighbor.f:
				addToOpen = False

			# Sometimes nodes that are already in the closed list are added to the open list nonetheless.
			inClosed = isPositionInClosed(neighbor, closedList)
			print("in closed:", inClosed)
			# Its probably due to the comparison between costs f < f.
			if inClosed != None and inClosed.f < neighbor.f:
				addToOpen = False

			if addToOpen:
				openList.put(neighbor, neighbor.f)
				print("open:", openList)

			# End of [closed-list] error section.

			input("\nPress Enter to continue...\n")
		print("end neighbors\n")

		closedList.append(currentNode)
		print("closed:", closedList)
		input("\nPress Enter to continue...\n")

	return reconstructPath(closedList)