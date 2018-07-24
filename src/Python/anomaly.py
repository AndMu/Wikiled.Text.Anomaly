from numpy import genfromtxt
from sklearn import preprocessing
import numpy as np
from pyod.models.knn import KNN   # kNN detector
from pyod.utils.data import evaluate_print

if __name__ == "__main__":
    data = genfromtxt('c:/1/vector.csv', delimiter=',')
    data = np.nan_to_num(data)
    # fit = np.isnan(data)
    # print(np.argwhere(np.isnan(data)))
    # print(np.any(np.isnan(data)))
    # print(np.all(np.isfinite(data)))
    x_scaled = preprocessing.scale(data)

    clf_name = 'My'
    clf = KNN()
    clf.fit(x_scaled)
    # get the prediction label and outlier scores of the training data
    y_train_pred = clf.labels_  # binary labels (0: inliers, 1: outliers)
    y_train_scores = clf.decision_scores_  # raw outlier scores

    X_test = x_scaled
    # get the prediction on the test data
    y_test_pred = clf.predict(X_test)  # outlier labels (0 or 1)
    y_test_scores = clf.decision_function(X_test)  # outlier scores

    # evaluate and print the results
    print("\nOn Training Data:")
    evaluate_print(clf_name, y_train, y_train_scores)
    print("\nOn Test Data:")
    evaluate_print(clf_name, y_test, y_test_scores)

    visualize(clf_name, X_train, y_train, X_test, y_test, y_train_pred,
              y_test_pred, show_figure=True, save_figure=False)
