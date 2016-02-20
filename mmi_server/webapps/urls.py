from django.conf.urls import patterns, include, url

urlpatterns = patterns('',
	url(r'^', include('mmi_server.urls')),
	)
