#!/usr/bin/env bash
set -euo pipefail

echo "Deleting namespace fcg-catalog"
kubectl delete namespace fcg-catalog --ignore-not-found=true
