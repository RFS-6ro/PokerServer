﻿namespace UniCastCommonData.ServerPool
{
#if false
	public interface IServerProcessFactory<SERVER_PROCESS, PARAM>
		where SERVER_PROCESS : class, IServerProcess
	{
		SERVER_PROCESS CreateWithParams(PARAM param);
	}
#endif
}
