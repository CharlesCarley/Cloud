# -----------------------------------------------------------------------------
#
#   Copyright (c) Charles Carley.
#
#   This software is provided 'as-is', without any express or implied
# warranty. In no event will the authors be held liable for any damages
# arising from the use of this software.
#
#   Permission is granted to anyone to use this software for any purpose,
# including commercial applications, and to alter it and redistribute it
# freely, subject to the following restrictions:
#
# 1. The origin of this software must not be misrepresented; you must not
#    claim that you wrote the original software. If you use this software
#    in a product, an acknowledgment in the product documentation would be
#    appreciated but is not required.
# 2. Altered source versions must be plainly marked as such, and must not be
#    misrepresented as being the original software.
# 3. This notice may not be removed or altered from any source distribution.
# ------------------------------------------------------------------------------
include(FindDoxygen)


# expects configuration parameters
# Docs_ProjectName    | STRING
# Docs_ProjectVersion | INT
# Docs_SourceLocation | DIRECTORY_LIST
# Docs_SourceExclude  | DIRECTORY_LIST
# Docs_SymbolExclude  | STRING_LIST
# Docs_ExamplePaths   | STRING_LIST
# Docs_ImagePaths     | STRING_LIST
# Docs_GenGraphs      | BOOL
# Docs_GenGraphsDot   | BOOL

if (DOXYGEN_FOUND)
	macro(MakeQuotedStringList OUT)
		set(${OUT} )
		set(WhiteSpace "                         ")
		foreach(str ${ARGN})
			set(${OUT} "${${OUT}}\"${str}\" \\\n${WhiteSpace}")
		endforeach()
	endmacro(MakeQuotedStringList)

	macro(MakeStringList OUT)
		set(${OUT} )
		set(WhiteSpace "                         ")
		foreach(str ${ARGN})
			set(${OUT} "${${OUT}}${str} \\\n${WhiteSpace}")
		endforeach()
	endmacro(MakeStringList)


	if (NOT EXISTS "${CMAKE_SOURCE_DIR}/docs")
		file(MAKE_DIRECTORY "${CMAKE_SOURCE_DIR}/docs")
	endif()

	set(TargetName Documentation)

	set(Input_CFG	 ${CMAKE_CURRENT_SOURCE_DIR}/Doxyfile.txt.in)
	set(Output_CFG	 ${CMAKE_CURRENT_BINARY_DIR}/Doxyfile)
	set(Layout_CFG	 ${CMAKE_CURRENT_SOURCE_DIR}/Layout.xml)
	set(Output_MAIN	 ${CMAKE_SOURCE_DIR}/docs/index.html)

	MakeQuotedStringList(Docs_SourceLocation ${Docs_SourceLocation})
	MakeQuotedStringList(Docs_SourceExclude  ${Docs_SourceExclude})
	MakeStringList(Docs_SymbolExclude        ${Docs_SymbolExclude})
	MakeQuotedStringList(Docs_ExamplePaths   ${Docs_ExamplePaths})
	MakeQuotedStringList(Docs_ImagePaths     ${Docs_ImagePaths})
	
	if (Docs_GenGraphs)
		unset(Docs_GenGraphs)
		set(Docs_GenGraphs "YES")
	else()
		set(Docs_GenGraphs "NO")
	endif()

	if (Docs_GenGraphsDot)
		unset(Docs_GenGraphs)
		unset(Docs_GenGraphsDot)
		set(Docs_GenGraphs "YES")
		set(Docs_GenGraphsDot "YES")
	else()
		set(Docs_GenGraphsDot "NO")
	endif()
	
	set_source_files_properties(${Output_MAIN} PROPERTIES GENERATED TRUE)


	set(Target_HTML
		header.html
		footer.html
		dox.css
		navtree.css
		style.css	
	)
	


	# This is meant to be embedded into another project
	# RootProject
	#  -- Documentation
	#    -- Extern -> This CMakeLists.txt
	#  -- Source
	#  -- Images
	# 

	file(GLOB Input_SRC ${CMAKE_CURRENT_SOURCE_DIR}/../Source/*.inl)
	file(GLOB Image_SRC   
			"${CMAKE_CURRENT_SOURCE_DIR}/../Images/*.png"
			"${CMAKE_CURRENT_SOURCE_DIR}/../Images/*.jpg"
			"${CMAKE_CURRENT_SOURCE_DIR}/../Images/*.svg"
			"${CMAKE_CURRENT_SOURCE_DIR}/../Images/*.dot"
			)

	configure_file(${Input_CFG} ${Output_CFG})

	set(Target_SRC
		${Target_HTML}
		${Layout_CFG}
		${Input_CFG}
		${Output_CFG}
		${Output_MAIN}
		${Input_SRC}
		${Image_SRC}
	)

	
	if (MSVC)
		source_group("HTML"   FILES ${Target_HTML} ${Layout_CFG})
		source_group("Config"    FILES ${Input_CFG}   ${Output_CFG})
		source_group("Source"    FILES ${Input_SRC})
		source_group("Resources" FILES ${Image_SRC})
	endif()

	add_custom_target(${TargetName}
		COMMAND ${DOXYGEN_EXECUTABLE} ${Output_CFG}
		WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}/docs 
		COMMENT "Building Documentation" 
		SOURCES ${Target_SRC} ${Input_SRC}
		)


	add_custom_command(
		TARGET ${TargetName}
		POST_BUILD
		COMMAND ${CMAKE_COMMAND} -E copy "${CMAKE_CURRENT_SOURCE_DIR}/navtree.css"
		 "${CMAKE_SOURCE_DIR}/docs/navtree.css"
	)
	set_target_properties(${TargetName} PROPERTIES FOLDER "CMakePredefinedTargets")
else()
	message("DOXYGEN not found")
endif()
