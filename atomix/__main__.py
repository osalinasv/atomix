from PIL import Image
from a_star import *


def loadImage(path : str):
	image = Image.open(path)
	return image.load() if image else [], image.size

def readWallsFromImage(pixels, width : int, height: int) -> []:
	walls = []

	for y in range(0, height):
		for x in range(0, width):
			if pixels[x, y] == (0, 0, 0):
				walls.append((x, y))

	return walls

def readAtomsFromImage(pixels, width : int, height: int) -> []:
	atoms = [()] * 5

	for y in range(0, height):
		for x in range(0, width):
			pixel = pixels[x, y]

			if pixel == (0, 0, 255):
				atoms[0] = (x, y)
			elif pixel == (0, 255, 255):
				atoms[1] = (x, y)
			elif pixel == (255, 0, 0):
				atoms[2] = (x, y)
			elif pixel == (255, 255, 0):
				atoms[3] = (x, y)
			elif pixel == (0, 255, 0):
				atoms[4] = (x, y)

	return atoms

def main(args = None):
	print("A* Atomix\n\n")

	pixels, size = loadImage("./data/diagram.png")
	width, height = size

	walls = readWallsFromImage(pixels, width, height)
	start = readAtomsFromImage(pixels, width, height)

	pixels, size = loadImage("./data/solution.png")
	width, height = size

	end = readAtomsFromImage(pixels, width, height)

	diagram = Diagram(width, height, walls = walls, start = start, end = end)

	diagram.initNodes()
	diagram.drawDiagram()

	path = a_star(diagram, diagram.getNode(start[0]), diagram.getNode(end[0]))
	print(path[0])

if __name__ == '__main__':
	main()