#!/bin/zsh
WORKSPACE=..

GEN_CLIENT=${WORKSPACE}/Tools/Luban.ClientServer/Luban.ClientServer.dll
CONF_ROOT=${WORKSPACE}/MiniDesignerConfigs

echo ${GEN_CLIENT}

dotnet ${GEN_CLIENT} -j cfg --\
 --define_file ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_code_dir Assets/Gen \
 --output_data_dir ../GenerateDatas/json \
 --service all \
 --gen_types "code_cs_unity_json,data_json"

read -p "Press enter to continue"
