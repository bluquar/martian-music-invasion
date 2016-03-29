from django.conf.urls import patterns, include, url
from mmi_server import views

urlpatterns = [
    url(r'^log/(?P<argument>.*)$', views.log, name='log'),
]
