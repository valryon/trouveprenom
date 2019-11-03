
echo "Archiving"

git archive --format zip --output ./trouveprenom.zip master

echo "Uploading"

scp -r trouveprenom.zip root@odin:/tmp

echo "Cleaning"

del trouveprenom.zip

echo "Ready to deploy."

pause