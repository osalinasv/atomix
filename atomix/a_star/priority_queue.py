import heapq


class PriorityQueue:
	def __init__(self):
		self._queue = []
		self._index = 0

	def __str__(self):
		return str([item[-1] for item in self._queue])

	def __repr__(self):
		return str([item[-1] for item in self._queue])

	def find(self, item):
		for q in self._queue:
			if item == q[-1]:
				return q[-1]
		return None

	def isEmpty(self):
		return len(self._queue) == 0

	def put(self, item, priority):
		heapq.heappush(self._queue, (priority, self._index, item))
		self._index += 1

	def pop(self):
		return heapq.heappop(self._queue)[-1]