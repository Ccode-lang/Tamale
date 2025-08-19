
file = open("./test.obj")
lines = file.readlines()
file.close()

vert = []
tex = []
faces = []

for line in lines:
    split = line.strip().split()

    if len(split) == 0:
        continue

    if split[0] == "v":
        vert += [split[1:]]
    elif split[0] == "vt":
        tex += [split[1:]]
    elif split[0] == "f":
        vert1 = split[1].split("/")
        vert2 = split[2].split("/")
        vert3 = split[3].split("/")
        faces += [[vert1, vert2, vert3]]

file = open("out.cof", "a")

for face in faces:
    #vert1        vert start                                                                                   tex start
    file.write(f"{vert[int(face[0][0]) - 1][0]},{vert[int(face[0][0]) - 1][1]},{vert[int(face[0][0]) - 1][2]},{tex[int(face[0][1]) - 1][0]},{tex[int(face[0][1]) - 1][1]}\n")

    file.write(f"{vert[int(face[1][0]) - 1][0]},{vert[int(face[1][0]) - 1][1]},{vert[int(face[1][0]) - 1][2]},{tex[int(face[1][1]) - 1][0]},{tex[int(face[1][1]) - 1][1]}\n")

    file.write(f"{vert[int(face[2][0]) - 1][0]},{vert[int(face[2][0]) - 1][1]},{vert[int(face[2][0]) - 1][2]},{tex[int(face[2][1]) - 1][0]},{tex[int(face[2][1]) - 1][1]}\n")
file.close()