// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#ifdef PLATFORM_WINDOWS
#include "gcenv.windows.inl"
#elif PLATFORM_UNIX
#include "gcenv.unix.inl"
#else
#error "gcenv not defined for platform"
#endif
