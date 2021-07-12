#!/bin/sh

#  CreateMDXFile.sh
#  MvpnTestIOSApp
#
#  Copyright © Citrix Systems, Inc. All rights reserved.

echo "pwd :: `pwd`"
PROJECT_DIR=`pwd`

export STOREURL="http://yourstore.yourdomain.com"
export APPTYPE="sdkapp"
# 1. PACKAGEID - Your UUID generated using the uuidgen tool in the macOS terminal.
export PACKAGEID=""
# 2. APPIDPREFIX - App ID Prefix (Team ID) found in your Apple Developer account.
export APPIDPREFIX=""
# 3. CERTNAME - Your codesign identity.
export CERTNAME=""
export TOOLKIT_DIR="$PROJECT_DIR/../../Tools/iOSTools"

echo $STOREURL
echo $APPTYPE
echo $PACKAGEID
echo $APPIDPREFIX
echo $TOOLKIT_DIR

if [ -z "${PACKAGEID}" ]
then
    echo "PACKAGEID variable was not found or was empty, please run uuidgen at the command line and paste the output value in the PACKAGEID variable in your post build script."
    exit 1
fi

if [ -z "${APPIDPREFIX}" ]
then
    echo "APPIDPREFIX variable was not found or was empty, please refer to the \"how to\" document located in the documentation folder of the SDK package on where to find your Apple's application prefix ID."
    exit 1
fi

if [ -z "${CERTNAME}" ]
then
    echo "CERTNAME variable was not found or was empty, please add your desired codesign identity."
    exit 1
fi

if [ ! -d "${TOOLKIT_DIR}" ]
then
    echo "TOOLKIT_DIR was not found, replace with your \"iOSTools\" path."
    exit 1
fi

if [ ! -d $TOOLKIT_DIR/logs ]
then
    mkdir "$TOOLKIT_DIR/logs"
fi


ENTITLEMENTS_PATH="$SRCROOT/$PROJECT/$PROJECT.entitlements"

APP_BUNDLE_PATH=`find . -name "*.app" | head -1`
if [[ $(find . -name "*.app" | wc -l) -ge 2 ]];then
    echo "More than 1 app bundle found, getting the last modified app bundle."
    APP_BUNDLE_PATH=`find . -name "*.app" -print0 | xargs -0 ls -t | head -1 | sed -e 's/://'`
fi
APP_BUNDLE_NAME=$(basename -- "${APP_BUNDLE_PATH}")
EXECUTABLE_NAME="${APP_BUNDLE_NAME%.*}"
MDX_PACKAGE_PATH="${APP_BUNDLE_PATH/.app/.mdx}"
EXECUTABLE_PATH="${APP_BUNDLE_PATH}/${EXECUTABLE_NAME}"

ENTITLEMENTS_PATH="Entitlements.plist"

echo "$TOOLKIT_DIR/CGAppCLPrepTool" SdkPrep -in "${APP_BUNDLE_PATH}" -out "${MDX_PACKAGE_PATH}" -storeURL "${STOREURL}" -appType "${APPTYPE}" -packageId "${PACKAGEID}" -entitlements $ENTITLEMENTS_PATH -appIdPrefix "${APPIDPREFIX}" -minPlatform "9.0"
"$TOOLKIT_DIR/CGAppCLPrepTool"      SdkPrep -in "${APP_BUNDLE_PATH}" -out "${MDX_PACKAGE_PATH}" -storeURL "${STOREURL}" -appType "${APPTYPE}" -packageId "${PACKAGEID}" -entitlements $ENTITLEMENTS_PATH -appIdPrefix "${APPIDPREFIX}" -minPlatform "9.0"

echo /usr/bin/codesign --force --sign "${CERTNAME}" --timestamp=none  "${EXECUTABLE_PATH}" --preserve-metadata=requirements,entitlements,identifier,flags,runtime
/usr/bin/codesign      --force --sign "${CERTNAME}" --timestamp=none  "${EXECUTABLE_PATH}" --preserve-metadata=requirements,entitlements,identifier,flags,runtime
