#!/bin/sh
rm -rf `find . -name __pycache__`
rm -f `find . -type f -name '*.py[co]' `
rm -f `find . -type f -name '*~' `
rm -f `find . -type f -name '.*~' `
rm -rf .cache
rm -rf .pytest_cache
rm -rf .idea
rm -rf venv
rm -rf bin obj packages
