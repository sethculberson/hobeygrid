import requests
from bs4 import BeautifulSoup
import pandas as pd

base_url = "https://www.eliteprospects.com/league/ncaa/stats/2019-2020?page="
columns=['ID','Player', 'Team', 'GP', 'G', 'A','TP','PPG','PIM','PM']

total_data = []

def getPageData(page):

    url = f"{base_url}{page}"
    response = requests.get(url)
    soup = BeautifulSoup(response.content, 'html.parser')
    for i in range(2, 12):
        body = soup.find_all('tbody')[i]
        for row in body.find_all('tr'):
            cells = [td.get_text(strip=True) for td in row.find_all('td')]
            #cells = pd.DataFrame([cells])
            #print(cells)
            total_data.append(cells)
    #return df

for i in range(1,16):
    getPageData(i)
    print(f"Page {i} added...")

df = pd.DataFrame(data=total_data, columns=columns)
df.to_csv('ncaa_stats_2019_2020.csv', index=False)