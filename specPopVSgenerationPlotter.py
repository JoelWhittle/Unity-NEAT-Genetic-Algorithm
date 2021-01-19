# -*- coding: utf-8 -*-
"""
Created on Sat Jan  9 10:56:55 2021

@author: Joel
"""

import matplotlib.pyplot as plt
import csv
from cycler import cycler

import pandas as pd
fig, ax = plt.subplots()
no = "161194412"
csv = pd.read_csv("C:\FlappyLogs\Primary\spec6\MEMBERSCOUNT" + str(no) + ".csv",  header=None)
# use skiprows if you want to skip headers
df_csv = pd.DataFrame(data=csv)
transposed_csv = df_csv

test = []
count = 0
for d in transposed_csv:
    t = []
    for n in transposed_csv[count]:
        t.append(n)
    test.append(t) 
    print(t)

    count += 1

labs = []
for n in range(0,count -1):
        labs.append("Species " + str(n))
        
ax.stackplot(range(1,51),test,  labels=labs)  



plt.suptitle('Species Member Count VS Generations Simulated in a population of NEAT agents.')
plt.ylabel('Number Of Members')
plt.xlabel('Generation')

plt.rc('axes', prop_cycle=(cycler('color', ['r', 'g', 'b', 'y','c','m','k','indigo','dimgray','rosybrown','lightcoral','maroon','tomato','orangered','sienna','olive','lawngreen','darkgreen','darkslategray','teal','steelblue','darkviolet'])))


ax.legend(fancybox=True, framealpha=.0, shadow=True, title = "Species Legend", loc='best', bbox_to_anchor=(0.75, -0.20),ncol=3)


plt.show()