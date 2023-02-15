import joblib
import time

def make_prediction(model, distance, nbJerks, nbLookDisruptive, OnRoadRate, meanSpeed):
    model = joblib.load(model)
    ease = model.predict([[distance, nbJerks, nbLookDisruptive, OnRoadRate, meanSpeed]])
    return int(ease[0])

def run_prediction():
    while True:
        with open('data_in.txt', 'r') as file:
            line = file.readline().strip() # Enlever le caractère de saut de ligne en fin de ligne
            data = [float(num) for num in line.split()]
        
        ease = make_prediction("svm.pkl", data[0], data[1], data[2], data[3], data[4])
        with open("predict.txt", "w") as f:
            f.write(str(ease))
        
        time.sleep(1)

run_prediction()
