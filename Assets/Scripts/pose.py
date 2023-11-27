import cv2
from cvzone.PoseModule import PoseDetector
import socket

host, port = "127.0.0.1", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((host, port))
cap = cv2.VideoCapture(0)

detector = PoseDetector()
while True:
    success, img = cap.read()
    img = detector.findPose(img)
    lmList, bboxInfo = detector.findPosition(img)

    if bboxInfo:
        lmString = ''
        for lm in lmList:
            lmString += f'{lm[1]},{img.shape[0] - lm[2]},{lm[3]},'
        sock.sendall(lmString.encode('utf-8'))
        receivedData = sock.recv(1024).decode("UTF-8")

    cv2.imshow("Image", img)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
# cap2.release()
cv2.destroyAllWindows()