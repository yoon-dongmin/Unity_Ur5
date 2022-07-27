from cgitb import reset


n = 3
lost = [3]

reserve = [1]

inx = 0
num = len(lost)
for i in reserve:
    flag = False
    if i+1 in lost:
        flag = True
        lost.remove(i+1)
    if i-1 in lost:
        flag = True
        lost.remove(i-1)
    if i in lost:
        flag = True
        lost.remove(i)

    if flag == True:
        inx += 1 

print(n - num + inx)
