//================================================================================================================================
//
//  Copyright (c) 2015-2021 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

#ifndef __MYCAMERA_H__
#define __MYCAMERA_H__

extern "C" {
    typedef struct
    {
        void * _state;
        void (* func)(void * _state, void * data, int len);
        void (* destroy)(void * _state);
    } FunctorOfVoidFromRawPointerOfVoidAndInt;

    bool openCamera(/* OUT */ void * * Return);
    void closeCamera(void * This);
    int getImageWidth(void * This);
    int getImageHeight(void * This);
    int getCameraType(void * This);
    int getOrientation(void * This);
    double getTimestamp(void * This);
    bool startCamera(void * This, FunctorOfVoidFromRawPointerOfVoidAndInt update);
    void stopCamera(void * This);
}

#endif
