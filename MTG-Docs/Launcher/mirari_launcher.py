from zipfile import ZipFile
import requests
import gdown
import time
import os
import sys

# Global variables
bundle_dir = os.path.abspath(os.path.dirname(__file__))
remoteInfoPath = os.path.join(bundle_dir, 'remote_version_info.txt')
versionInfoPath = os.path.join(bundle_dir, 'version_info.txt')
outputPath = os.path.join(bundle_dir, 'client.zip')
remoteVersionURL = 'https://raw.githubusercontent.com/kenzo-bt/mirari-release/refs/heads/main/release_info.txt'

# Functions
def start():
    remote_version, remote_url = get_release_info()
    if remote_version != False:
        # Compare remote version with current
        file = open(versionInfoPath, 'r')
        version, url = file.read().split(' ')
        file.close()
        if version != remote_version or url != remote_url:
            # Update the client
            update_client(remote_version, remote_url)
        launch()
    else:
        print('Error: Could not retrive version information')

def get_release_info():
    print("Fetching client version from server...")
    response = requests.get(remoteVersionURL)
    if response.status_code == 200:
        with open(remoteInfoPath, 'wb') as file:
            file.write(response.content)
        file = open(remoteInfoPath, 'r')
        version, url = file.read().split(' ')
        file.close()
        return version, url
    else:
        return False

def update_client(version, url):
    print("Update required. Downloading client from server...")
    # Download and extract client
    gdown.download(url=url, output=outputPath, fuzzy=True)
    print("Extracting files...")
    with ZipFile(outputPath, 'r') as zObject:
        zObject.extractall(path=os.path.join(bundle_dir, 'client'))
    zObject.close()
    os.remove(outputPath)
    # Update version information
    file = open(versionInfoPath, 'w')
    file.write(version + ' ' + url)
    file.close()

def launch():
    print('Launching game...')
    exe_path = os.path.join(bundle_dir, 'client\\DecoupledBuild\\Mirari.exe')
    os.startfile(exe_path)
    sys.exit()

start()
