#!/bin/sh

#  EmbedIPA.sh
#
#  For Enterprise app deployment.
#
#  Copyright © Citrix Systems, Inc. All rights reserved.

echo "pwd :: `pwd`"
PROJECT_DIR=`pwd`

export TOOLKIT_DIR="$PROJECT_DIR/../../Tools/iOSTools"

echo $TOOLKIT_DIR

if [ ! -d "${TOOLKIT_DIR}" ]
then
    echo "TOOLKIT_DIR was not found, replace with your \"iOSTools\" path."
    exit 1
fi

if [ ! -d $TOOLKIT_DIR/logs ]
then
    mkdir "$TOOLKIT_DIR/logs"
fi

MDX_FILE_PATH=`find . -name "*.mdx" | head -1`
if [[ $(find . -name "*.mdx" | wc -l) -ge 2 ]];then
    echo "More than 1 mdx found, getting the last modified mdx."
    MDX_FILE_PATH=`find . -name "*.mdx" -print0 | xargs -0 ls -t | head -1 | sed -e 's/://'`
fi

if [ ! -f "${MDX_FILE_PATH}" ]
then
    echo "MDX_FILE_PATH was not found, please generate an MDX file as part of your Visual Studio for Mac build"
    exit 1
fi

IPA_FILE_PATH=`find . -name "*.ipa" | head -1`
if [[ $(find . -name "*.ipa" | wc -l) -ge 2 ]];then
    echo "More than 1 ipa found, getting the last modified ipa."
    IPA_FILE_PATH=`find . -name "*.ipa" -print0 | xargs -0 ls -t | head -1 | sed -e 's/://'`
fi

if [ ! -f "${IPA_FILE_PATH}" ]
then
    echo "IPA_FILE_PATH was not found, please generate an IPA as part of your Visual Studio for Mac build"
    exit 1
fi

EMBEDDED_MDX_PATH="${IPA_FILE_PATH/.ipa/.embedded.mdx}"

"$TOOLKIT_DIR/CGAppCLPrepTool" SetInfo -in "$MDX_FILE_PATH" -out "$EMBEDDED_MDX_PATH" -embedBundle "$IPA_FILE_PATH"