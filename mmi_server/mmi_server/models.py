from django.db import models
from django.contrib.auth.models import User

class UserProfile(models.Model):
    """ Additional profile data attached to a User """
    user = models.OneToOneField(User)
    following = models.ManyToManyField(User, related_name='followers')
    bio = models.CharField(max_length=430, null=True)
    profile_picture_url = models.CharField(null=True, max_length=256)

class LogFile(models.Model):
    session_id = models.CharField(max_length=48)
    filename = models.CharField(max_length=64)


