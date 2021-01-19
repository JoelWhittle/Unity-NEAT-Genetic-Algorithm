# -*- coding: utf-8 -*-
"""
Created on Sat Jan  9 10:56:55 2021

@author: Joel
"""

import matplotlib.pyplot as plt
import csv
from cycler import cycler
import math

import pandas as pd
fig, ax = plt.subplots()
no = "161194412"
csv = pd.read_csv("C:\FlappyLogs\Primary\spec6\FITNESS" + str(no) + ".csv",  header=None)
# use skiprows if you want to skip headers
df_csv = pd.DataFrame(data=csv)
transposed_csv = df_csv

colCount = transposed_csv.shape[1]
averageList = []
count = 0
for d in transposed_csv:
    t = []
    g = 0
    m = []

    for n in transposed_csv[count]:
        t.append(n)
       
        m.append(n)
      
        g +=1
    total = 0
    for index in m :
        total += index
    total /= len(m)    
    
    labelText = 'Species ' + str(count)
    if(count == colCount -2):
       labelText = "Average Fitness of Population"
    if(count == colCount -1):
        labelText = ''

    ax.plot(t,   label=labelText)   
    count += 1



count = 0
transposed_csv = df_csv.T

for d in transposed_csv:
    t = []
    g = 0
    m = []

    for n in transposed_csv[count]:
   
        m.append(n)
      
        g +=1
    total = 0
    activeSpec = 0
    
    mIndex = 0
    for index in m:
      if mIndex < len(m) -1: 
       mIndex +=1
       if math.isnan(index) == False:
           if index > 0:
               total += index
               activeSpec += 1
    if activeSpec > 0:           
        total /= activeSpec    
        
    averageList.append(total)
    count += 1


plt.plot(averageList, label="Average Fitness of active species")
plt.suptitle('Species Average Fitness VS Generations Simulated in a population of NEAT agents.')
plt.ylabel('Average Fitness Of Species')
plt.xlabel('Generation')

plt.rc('axes', prop_cycle=(cycler('color', ['r', 'g', 'b', 'y','c','m','k','indigo','dimgray','rosybrown','lightcoral','maroon','tomato','orangered','sienna','olive','lawngreen','darkgreen','darkslategray','teal','steelblue','darkviolet'])))


ax.legend(fancybox=True, framealpha=.0, shadow=True, title = "Species Legend", loc='best', bbox_to_anchor=(0.75, -0.20),ncol=3)


plt.show()