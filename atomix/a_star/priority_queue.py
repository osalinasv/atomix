import heapq


class PriorityQueue:
	def __init__(self):
		self.elements = []
		self.set = []

	def empty(self):
		return len(self.elements) == 0

	def put(self, item, priority):
		self.elements.append(item)
		heapq.heappush(self.set, (priority, item))

	def get(self):
		item = heapq.heappop(self.set)[1]
		self.elements.remove(item)
		return item