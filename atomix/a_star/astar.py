from a_star import Diagram
from a_star import Node
from a_star import PriorityQueue

def manhattanHeuristic(a : Node, b: Node) -> float:
	return abs(a.position[0] - b.position[0]) + abs(a.position[1] - b.position[1])

def tracePath(start : Node, end : Node) -> []:
	path = []
	current : Node = end

	while current != start:
		if current.parent:
			path.append(current.parent)
			current = current.parent
		else:
			break

	return path

def a_star(diagram : Diagram, start : Node, end : Node) -> []:
	openList = PriorityQueue()
	closedList = []

	start.setCost(0, manhattanHeuristic(start, end))
	openList.put(start, start.f)

	while not openList.isEmpty():
		currentNode = openList.pop()
		closedList.append(currentNode)
		print("CURRENT:", currentNode)

		if currentNode == end:
			return tracePath(start, end)

		neighbours = diagram.getNeighbours(currentNode)
		print("NEIGHBOURS for CURRENT:", neighbours, "\n")
		for neighbour in neighbours:
			if neighbour in closedList:
				print("neighbour was in closed list.")
				continue

			cost = currentNode.cost + manhattanHeuristic(currentNode, neighbour)
			inOpen = openList.find(neighbour)
			if cost < neighbour.cost or inOpen == None:
				heuristic = manhattanHeuristic(neighbour, end)
				neighbour.setCost(cost, heuristic)
				neighbour.parent = currentNode

				if inOpen == None:
					openList.put(neighbour, neighbour.f)
					print("n:", neighbour, "\t", "c:", neighbour.cost, "\t", "h:", neighbour.heuristic, "\t", "f:", neighbour.f)
					print("open:", openList)
			else:
				print("new cost bigger or in open list.")

			# input("\nPress Enter to continue...\n")

		print("CURRENT PATH:", tracePath(start, currentNode))
		input("\nPress Enter to continue...\n")

	return tracePath(start, end)