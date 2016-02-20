from django.conf.urls import patterns, include, url

urlpatterns = patterns('',
                       url(r'^log/(?P<argument>.*)$', 'mmi_server.views.log', name='log'),
                       )

"""


	url(r'^$','mmi_server.views.home', name='home'),
	url(r'^browse$', 'mmi_server.views.browse', name="browse"),
	url(r'^user/(?P<username>.*)$', 'mmi_server.views.user', name="user"),

	# Async database updates
	url(r'^new_definition$', 'mmi_server.views.new_definition', name="new_definition"),

	# Account management
	url(r'^login$', 'django.contrib.auth.views.login', {'template_name':'mmi_server/login.html'}, name="login"),
	url(r'^register$', 'mmi_server.views.register', name="register"),
	url(r'^logout$', 'django.contrib.auth.views.logout_then_login', name="logout"),
"""
