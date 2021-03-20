# import tensorflow as tf
import numpy as np
import os
import resampy
import random
import hdf5storage # zum Lesen von grossen .mat Datein
import shutil
from scipy import signal


class Read():

    # Initialisieren leere Listen und mit "self" haben wir eine Art globalen Zugriff auf diese Listen
    def __init__(self):
        self.Gyro = []
        self.ACC = []
        self.AUX = []
        self.Data_Meta_start_end_durataion = []
        self.timestampIMU = []
        self.Data_Meta_label_timestamps = []
        self.Data_Meta_label_taskID = []

    def enroll(self, data):
        tmp_list = []
        for i in range(np.shape(data)[0]):
            tmp_data = data[i][0]
            tmp_list.append(tmp_data)
        return tmp_list

    def enroll_2ndOrder(self, data):
        tmp_list = []
        for i in range(np.shape(data)[0]):
            tmp_data = data[i][0][0]
            tmp_list.append(tmp_data)
        return tmp_list


    def add_data(self, matFilename):
        print ('Adding new data')
        fullFilename = matFilename[1]
        print (fullFilename)

        data = hdf5storage.loadmat(fullFilename)
        Data_ACC = self.enroll(data['ACC'])
        Data_AUX = self.enroll(data['AUX'])
        Data_GYR = self.enroll(data['GYR'])
        Data_Meta_start_end_durataion = data['Meta']['listDuration'][0][0]      # Das sind die Stamps, bei denen die jeweiligen Bewegungen anfangen
        Data_Meta_inception_lvl = data['Meta']['metaData']
        Data_Meta_label_timestamps = self.enroll_2ndOrder(Data_Meta_inception_lvl[0][0][0]['duration'])
        Data_Meta_label_taskID = self.enroll_2ndOrder(Data_Meta_inception_lvl[0][0][0]['taskID'])
        Data_timestampIMU = np.squeeze(data['timestampIMU'])

        self.ACC.append(Data_ACC)
        self.AUX.append(Data_AUX)
        self.Gyro.append(Data_GYR)
        self.Data_Meta_start_end_durataion.append(Data_Meta_start_end_durataion)
        self.timestampIMU.append(Data_timestampIMU)
        self.Data_Meta_label_timestamps.append(Data_Meta_label_timestamps)
        self.Data_Meta_label_taskID.append(Data_Meta_label_taskID)


    def load_data(self, dataset_rootfolder):
        fileList = []

        if isinstance(dataset_rootfolder, list):

            for rootfolder in dataset_rootfolder:
                fileListTmp = os.listdir(rootfolder + '/')

                for file in fileListTmp:
                    if file.endswith(".mat"):
                        fileList.append(rootfolder + file)
                        # print('P1:',fileList)

        else:
            fileListTmp = os.listdir(dataset_rootfolder)

            for file in fileListTmp:
                if file.endswith(".mat"):
                    fileList.append(dataset_rootfolder + file)
                    # fileList.append(dataset_rootfolder + '/' + file)  # fuer Win10 braucht man ein extra /
                    # print('P2:',fileList)

        for filename in enumerate(fileList):
            # print('Processing overall ' + str(len(fileList)) + ' files')
            self.add_data(filename)


# ------------------------------------------
# -------- Hier geht der Kram weiter -------
# ------------------------------------------
path = 'C:/Users/Mark/Desktop/UpdateD/'
# path = '../UpdateD/'
R = Read()
a = R.load_data(path)

'''
Holen den ganzen Kram aus der Klasse 'Read', damit man die Skripte später splitten kann
'''
Gyro = R.Gyro
ACC = R.ACC
AUX = R.AUX
Data_Meta_start_end_durataion = R.Data_Meta_start_end_durataion
timestampIMU = R.timestampIMU
Data_Meta_label_timestamps = R.Data_Meta_label_timestamps
Data_Meta_label_taskID = R.Data_Meta_label_taskID

label_shift_idx_previous = 0
def generate_Label_without_time_offset(Data_Meta_start_end_durataion,timestampIMU,Data_Meta_label_timestamps,Data_Meta_label_taskID, Gyro, sampling_rate : float = None ):
    '''
    Example for first .mat
    :param timestampIMU: list: 9230 length; 0-62.29sec
    :param Data_Meta_label_timestamps:  list: 17 length duration of every gesture
    :param Data_Meta_label_taskID: versteht sich wohl von selbst
    :param Gyro jede andere Modalität ist Schmutz
    :param final_sampling_rate: float: Zielsamplingrate
    :return:
    '''
    global label_shift_idx_previous

    Gyro_time_cropped_list = []
    timestampIMU_time_cropped_list = []
    label_vector_time_cropped_list = []

    # resampling if necessary

    for i in range(len(timestampIMU)):
        if sampling_rate is not None:
            delta_t = timestampIMU[i][1] - timestampIMU[i][0]
            sr_freq = 1 / delta_t
            if sr_freq != sampling_rate:
                Gyro_temp = Gyro[i]
                for j in range(len(Gyro_temp)):
                    Gyro_temp[j] = resampy.resample(x=Gyro_temp[j], sr_orig=sr_freq, sr_new=sampling_rate,axis=0)
                Gyro[i] = Gyro_temp
                timestampIMU[i] = np.arange(timestampIMU[i][0],timestampIMU[i][-1],1/sampling_rate)


    for i in range(np.shape(Data_Meta_start_end_durataion)[0]):
        label_vector_list = []
        label_shift_idx_previous = 0
        start_time = Data_Meta_start_end_durataion[i][1,0]  # lese hier einfach den Startpunkt aus, wenn in der GUI auch 'Start' getippt wurde
        end_time = Data_Meta_start_end_durataion[i][-2,1]
        start_idx = np.min(np.where(timestampIMU[i] >= start_time)[0])
        end_idx = np.max(np.where(timestampIMU[i] <= end_time)[0])
        timestampIMU_i = timestampIMU[i][start_idx:]
        Gyro_i = np.asarray(Gyro[i])[:,start_idx:end_idx,:]

        label_timestamps = Data_Meta_label_timestamps[i]
        for j in range(0,np.shape(label_timestamps)[0]):
            sum_time = np.cumsum(Data_Meta_label_timestamps[i][:(j+1)])[-1]

            label_shift_idx = np.min(np.where(timestampIMU[i] >= sum_time)[0])
            label_id = Data_Meta_label_taskID[i][j]
            label_vector = np.zeros((label_shift_idx-label_shift_idx_previous,), dtype=np.int32)+label_id
            label_shift_idx_previous = label_shift_idx
            label_vector_list.extend(label_vector)

        Gyro_time_cropped_list.append(Gyro_i)
        timestampIMU_time_cropped_list.append(timestampIMU_i)
        label_vector_time_cropped_list.append(label_vector_list)

    return Gyro_time_cropped_list, timestampIMU_time_cropped_list, label_vector_time_cropped_list

samplig_rate = 10.0
Gyro,timestampIMU,label = generate_Label_without_time_offset(Data_Meta_start_end_durataion,timestampIMU,Data_Meta_label_timestamps,Data_Meta_label_taskID,Gyro,samplig_rate)

# ------------------------------------------
# ------------ Initialisierung  ------------
# ------------------------------------------


print(Gyro)

print('stop')