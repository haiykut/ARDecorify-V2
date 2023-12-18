//================================================================================================================================
//
//  Copyright (c) 2015-2021 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

#include "MyCamera.h"

#import <AVFoundation/AVFoundation.h>
#import <Endian.h>
#import <UIKit/UIKit.h>

#include <chrono>
#include <memory>

@interface MyCamera : NSObject
@property (nonatomic, strong) AVCaptureSession *captureSession;
@property (nonatomic, strong) AVCaptureDeviceInput *deviceInput;
- (instancetype)init;
- (BOOL)open;
- (BOOL)start;
- (BOOL)stop;
-(CGSize)size;
@end

@interface MyCamera() < AVCaptureVideoDataOutputSampleBufferDelegate >
{
    bool opened_;
}
@property(nonatomic, weak) AVCaptureDevice *videoDevice;
@property(nonatomic, strong) NSMutableArray *allSupportedPreviewSize;
@property(nonatomic) CGSize previewSize;
@property(nonatomic, strong) NSArray *allSupportedFPS;
@property(nonatomic, strong) AVCaptureVideoDataOutput *captureOutPut;
@property(nonatomic) FunctorOfVoidFromRawPointerOfVoidAndInt cameraImageFrameUpdate;
@property(nonatomic) CMSampleBufferRef sampleBuffer;
@end

@implementation MyCamera {
    Byte *imageData;
}

- (instancetype)init
{
    self = [super init];
    
    if (self) {
        self.captureSession = [[AVCaptureSession alloc] init];
        self.captureOutPut = nil;
        imageData = nil;
        opened_ = false;
    }
    return self;
}

-(BOOL)open
{
    self.videoDevice = [self cameraWithPosition : AVCaptureDevicePositionBack];
    if (!self.videoDevice) {
        return FALSE;
    }
    
    NSError *error;
    AVCaptureDeviceInput *videoInput = [AVCaptureDeviceInput deviceInputWithDevice : self.videoDevice error : &error];
    if (error) {
        return FALSE;
    }
    [self setDeviceInput : videoInput];
    
    if ([self.captureSession canAddInput : videoInput]) {
        [self.captureSession addInput : videoInput];
    } else {
        NSLog(@"Could not add input port to capture session %@", self.captureSession);
    }
    
    [self addVideoDataOutput];
    opened_ = true;
    
    self.allSupportedPreviewSize = [[NSMutableArray alloc] init];
    
    if ([self.captureSession canSetSessionPreset : AVCaptureSessionPreset1280x720]) {
        [self.allSupportedPreviewSize addObject : AVCaptureSessionPreset1280x720];
    }
    if ([self.captureSession canSetSessionPreset : AVCaptureSessionPreset640x480]) {
        [self.allSupportedPreviewSize addObject : AVCaptureSessionPreset640x480];
    }
    if ([self.captureSession canSetSessionPreset : AVCaptureSessionPreset1920x1080]) {
        [self.allSupportedPreviewSize addObject : AVCaptureSessionPreset1920x1080];
    }
    if (self.allSupportedPreviewSize.count > 0) {
        CGSize defaultSize = [self getSupportedSize : 0];
        [self setSize : defaultSize];
    }
    
    return TRUE;
}

-(BOOL)start
{
    if (!self.captureSession) {
        return FALSE;
    }
    [self.captureSession startRunning];
    return TRUE;
}

-(BOOL)stop
{
    if (!self.captureSession) {
        return FALSE;
    }
    [self.captureSession stopRunning];
    return TRUE;
}

-(CGSize)size
{
    return self.previewSize;
}

-(int)getNumSupportedSize
{
    if (!opened_) {
        return 0;
    }
    return (int)(self.allSupportedPreviewSize.count);
}

-(CGSize)getSupportedSize:(int)idx
{
    if (!opened_ || idx < 0 || idx >[self getNumSupportedSize]) {
        return CGSizeMake(0, 0);
    }
    NSString *preset = [self.allSupportedPreviewSize objectAtIndex : idx];
    if ([preset isEqualToString : AVCaptureSessionPreset640x480]) {
        return CGSizeMake(640, 480);
    }
    
    if ([preset isEqualToString : AVCaptureSessionPreset1280x720]) {
        return CGSizeMake(1280, 720);
    }
    
    if ([preset isEqualToString : AVCaptureSessionPreset1920x1080]) {
        return CGSizeMake(1920, 1080);
    }
    
    return CGSizeMake(0, 0);
}

-(BOOL)setSize:(CGSize)size
{
    if (!opened_) {
        return FALSE;
    }
    CGSize optSize = [self getOptimalPreviewSize : size];
    for (NSString* preset in self.allSupportedPreviewSize) {
        if (([preset isEqualToString : AVCaptureSessionPreset640x480] && optSize.width == 640 && optSize.height == 480) ||
            ([preset isEqualToString : AVCaptureSessionPreset1280x720] && optSize.width == 1280 && optSize.height == 720) ||
            ([preset isEqualToString : AVCaptureSessionPreset1920x1080] && optSize.width == 1920 && optSize.height == 1080)) {
            if (optSize.width != self.previewSize.width || optSize.height != self.previewSize.height) {
                [self.captureSession setSessionPreset : preset];
                self.previewSize = optSize;
            }
            return TRUE;
        }
    }
    return FALSE;
}

-(CGSize)getOptimalPreviewSize:(CGSize)size
{
    long area = size.width * size.height;
    
    CGSize res = CGSizeMake(0, 0);;
    if (self.allSupportedPreviewSize.count > 0)
        res = [self getSupportedSize : 0];
    long minAreaDiff = std::numeric_limits<long>::max();
    for (int i = 0; i < [self getNumSupportedSize]; ++i) {
        CGSize sizei = [self getSupportedSize : i];
        long areaDiff = std::abs(sizei.width * sizei.height - area);
        
        if (areaDiff < minAreaDiff) {
            minAreaDiff = areaDiff;
            res = sizei;
        }
    }
    return res;
}

#pragma mark -
#pragma mark Helper Methods
- (AVCaptureDevice*)cameraWithPosition:(AVCaptureDevicePosition)position
{
    NSArray *devices = [AVCaptureDevice devicesWithMediaType : AVMediaTypeVideo];
    for (AVCaptureDevice *device in devices) {
        if ([device position] == position) {
            return device;
        }
    }
    return nil;
}

-(void)addVideoDataOutput
{
    self.captureOutPut = [[AVCaptureVideoDataOutput alloc] init];
    self.captureOutPut.alwaysDiscardsLateVideoFrames = YES;
    
    dispatch_queue_t queue;
    queue = dispatch_queue_create("cn.easyar.samples", DISPATCH_QUEUE_SERIAL);
    [self.captureOutPut setSampleBufferDelegate : self queue : queue];
    
    NSString *key = (NSString *)kCVPixelBufferPixelFormatTypeKey;
    NSNumber *value = [NSNumber numberWithUnsignedInt : kCVPixelFormatType_420YpCbCr8BiPlanarFullRange];
    NSDictionary *settings = @{key:value};
    [self.captureOutPut setVideoSettings : settings];
    
    [self.captureSession addOutput : self.captureOutPut];
}

-(void)captureOutput:(AVCaptureOutput *)captureOutput
didOutputSampleBuffer : (CMSampleBufferRef)sampleBuffer
     fromConnection : (AVCaptureConnection *)connection
{
    (void)captureOutput;
    (void)connection;
    
    self.sampleBuffer = sampleBuffer;
    
    CVImageBufferRef imageBuffer = CMSampleBufferGetImageBuffer(sampleBuffer);
    
    CVPixelBufferLockBaseAddress(imageBuffer, kCVPixelBufferLock_ReadOnly);
    
    size_t width = CVPixelBufferGetWidth(imageBuffer);
    size_t height = CVPixelBufferGetHeight(imageBuffer);
    
    uint8_t *yAddr = (uint8_t*)CVPixelBufferGetBaseAddressOfPlane(imageBuffer, 0);
    uint8_t *uvAddr = (uint8_t*)CVPixelBufferGetBaseAddressOfPlane(imageBuffer, 1);
    
    if(!yAddr || !uvAddr) return;
    
    int ySize = (int)(width * height);
    int uvSize = ySize/2;

    int size = (int)(width * height * 3 / 2);
    if(imageData == nil)
        imageData = (Byte*)malloc(size);
    memcpy(imageData, yAddr, ySize);
    memcpy(imageData + ySize, uvAddr, uvSize);
    CVPixelBufferUnlockBaseAddress(imageBuffer, kCVPixelBufferLock_ReadOnly);
    if(self.cameraImageFrameUpdate.func != nullptr)
    {
        self.cameraImageFrameUpdate.func(self.cameraImageFrameUpdate._state, imageData, size);
    }
}

-(void)dealloc
{
    if (self.cameraImageFrameUpdate.destroy != nullptr)
    {
        self.cameraImageFrameUpdate.destroy(self.cameraImageFrameUpdate._state);
        self.cameraImageFrameUpdate = {};
    }
    if (self.captureOutPut != nil)
        [self.captureSession removeOutput: self.captureOutPut];
    if(imageData != nil)
        free(imageData);
    [self.captureSession stopRunning];
}
@end

static MyCamera * instance;

bool openCamera(/* OUT */ void * * Return)
{
    auto instance = [[MyCamera alloc] init];
    *Return = (__bridge_retained void *)instance;
    return [instance open] != FALSE;
}

void closeCamera(void * This)
{
    auto instance = (__bridge_transfer MyCamera *)This;
    instance = nil;
    [instance stop];
}

int getImageWidth(void * This)
{
    auto instance = (__bridge MyCamera *)This;
    return instance.previewSize.width;
}

int getImageHeight(void * This)
{
    auto instance = (__bridge MyCamera *)This;
    return instance.previewSize.height;
}

int getCameraType(void * This)
{
    /*
    Default = 0,
    Back = 1,
    Front = 2,*/
    return 0;
}

int getOrientation(void * This)
{
    return 90;
}

double getTimestamp(void * This)
{
    auto instance = (__bridge MyCamera *)This;
    double timestamp = CMTimeGetSeconds(CMSampleBufferGetPresentationTimeStamp(instance.sampleBuffer));
    return timestamp;
}

bool startCamera(void * This, FunctorOfVoidFromRawPointerOfVoidAndInt update)
{
    auto instance = (__bridge MyCamera *)This;
    if (instance.cameraImageFrameUpdate.destroy != nullptr)
    {
        instance.cameraImageFrameUpdate.destroy(instance.cameraImageFrameUpdate._state);
    }
    instance.cameraImageFrameUpdate = update;
    return [instance start] != FALSE;
}

void stopCamera(void * This)
{
    auto instance = (__bridge MyCamera *)This;
    [instance stop];
}
